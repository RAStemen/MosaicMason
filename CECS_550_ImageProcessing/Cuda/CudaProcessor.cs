using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GASS.CUDA;
using GASS.CUDA.Types;
using System.IO;
using CECS_550_ImageProcessing;
using System.Windows.Forms;


namespace Cuda
{
    public class CudaProcessor
    {

        const int BLOCK_SIZE = 16;
        static int[] hMasterData, hCandidatesData;
        public static int[] hSolutionData;

        public static int[] CUDA_SolveMosaic(string masterImageArray, List<string> candidateImages, int subdivLevel, int miWidthInCells) // Using CUDADriver class static methods
        {

            InitializeData(masterImageArray, candidateImages, subdivLevel, miWidthInCells);

            // Initialize CUDA driver and select 1st device.
            CUDADriver.cuInit(0);
            CUdevice device;
            device.Pointer = 0;
            CUDADriver.cuDeviceGet(ref device, 0);

            // Create a context (i.e. a GPU process)
            CUcontext ctx;
            ctx.Pointer = IntPtr.Zero;
            CUDADriver.cuCtxCreate(ref ctx, 0, device);

            // Load a GPU module
            CUmodule module;
            module.Pointer = IntPtr.Zero;
            CUDADriver.cuModuleLoad(ref module, Path.Combine(Application.StartupPath,
                "MosaicSolver_Kernel.cubin"));

            // Get module kernel functions
            CUfunction solveMosaic;
            solveMosaic.Pointer = IntPtr.Zero;
            CUDADriver.cuModuleGetFunction(ref solveMosaic, module, "solveMosaic"); 

            // Create a stream for events to time the performance (using the GPU timer)
            CUstream stream;
            stream.Pointer = IntPtr.Zero;
            CUDADriver.cuStreamCreate(ref stream, 0);
            CUevent start;
            start.Pointer = IntPtr.Zero;
            CUDADriver.cuEventCreate(ref start, 0);
            CUevent end;
            end.Pointer = IntPtr.Zero;
            CUDADriver.cuEventCreate(ref end, 0);
            CUDADriver.cuEventRecord(start, stream);

            // Allocate device (GPU) memory and copy host (CPU) input memory to device
            CUdeviceptr dMaster, dCandidates, dSolution;
            dMaster.Pointer = 0;
            dCandidates.Pointer = 0;
            dSolution.Pointer = 0;
            CUDADriver.cuMemAlloc(ref dMaster, (uint)(sizeof(int) * hMasterData.Length));
            CUDADriver.cuMemcpyHtoD(dMaster, hMasterData, (uint)(sizeof(int) * hMasterData.Length));
            CUDADriver.cuMemAlloc(ref dCandidates, (uint)(sizeof(int) * hCandidatesData.Length));
            CUDADriver.cuMemcpyHtoD(dCandidates, hCandidatesData, (uint)(sizeof(int) * hCandidatesData.Length));
            CUDADriver.cuMemAlloc(ref dSolution, (uint)(sizeof(int) * hSolutionData.Length));
            CUDADriver.cuMemcpyHtoD(dSolution, hSolutionData, (uint)(sizeof(int) * hSolutionData.Length));

            // Set kernel function parameters
            int offset = 0;
            CUDADriver.cuParamSeti(solveMosaic, 0, (uint)dSolution.Pointer); offset += IntPtr.Size;
            CUDADriver.cuParamSeti(solveMosaic, offset, (uint)dMaster.Pointer); offset += IntPtr.Size;
            CUDADriver.cuParamSeti(solveMosaic, offset, (uint)dCandidates.Pointer); offset += IntPtr.Size;
            CUDADriver.cuParamSeti(solveMosaic, offset, (uint)subdivLevel); offset += sizeof(uint);
            CUDADriver.cuParamSeti(solveMosaic, offset, (uint)candidateImages.Count); offset += sizeof(uint);
            CUDADriver.cuParamSeti(solveMosaic, offset, (uint)miWidthInCells); offset += sizeof(uint);
            CUDADriver.cuParamSetSize(solveMosaic, (uint)offset);

            // Launch kernel function
            CUDADriver.cuFuncSetBlockShape(solveMosaic, BLOCK_SIZE, BLOCK_SIZE, 1);
            CUDADriver.cuLaunchGrid(solveMosaic, (miWidthInCells / BLOCK_SIZE), (miWidthInCells / BLOCK_SIZE));

            //CUDADriver.cuLaunchGrid(solveMosaic, miWidthInCells/ BLOCK_SIZE, miWidthInCells/BLOCK_SIZE);

            // Copy device (GPU) result to host (CPU) memory
            CUDADriver.cuMemcpyDtoH(hSolutionData, dSolution, (uint)(sizeof(int) * hSolutionData.Length));

            // Output GPU timing result 
            CUDADriver.cuEventRecord(end, stream);
            CUDADriver.cuStreamSynchronize(stream);
            float GpuTime = 0.0f;
            CUDADriver.cuEventElapsedTime(ref GpuTime, start, end);
            Console.WriteLine("GPU time:\t\t\t{0} ms", GpuTime);

            // Free device (GPU) resources
            CUDADriver.cuMemFree(dMaster);
            CUDADriver.cuMemFree(dCandidates);
            CUDADriver.cuMemFree(dSolution);
            CUDADriver.cuModuleUnload(module);
            CUDADriver.cuCtxDestroy(ctx);

            return hSolutionData;
        }

        private static void InitializeData(string masterImageArray, List<string> candidateImages, int subdivLevel, int miWidthInCells)
        {
            CudaProcessor.hMasterData = new int[3 * miWidthInCells * miWidthInCells * subdivLevel * subdivLevel];
            CudaProcessor.hCandidatesData = new int[3 * candidateImages.Count * subdivLevel * subdivLevel];
            CudaProcessor.hSolutionData = new int[candidateImages.Count * miWidthInCells * miWidthInCells];

            int length = masterImageArray.Length / 6;
            int[] color;
            int milength = masterImageArray.Length;

            //Setting up the Master image data on the host
            for (int i = 0; i < length; i++)
            {
                color = XmlHelper.RGBStringToIntArray(masterImageArray.Substring(i * 6, 6));
                CudaProcessor.hMasterData[(3 * i)] = color[0];
                CudaProcessor.hMasterData[(3 * i) + 1] = color[1];
                CudaProcessor.hMasterData[(3 * i) + 2] = color[2];
            }


            //Setting up the Candidate image data on the host
            int numCandidates = candidateImages.Count;
            int cellStride = 3 * subdivLevel * subdivLevel;
            string colors;
            for (int i = 0; i < numCandidates; i++)
            {
                colors = candidateImages[i];
                length = colors.Length / 6;
                for (int j = 0; j < length; j++)
                {
                    color = XmlHelper.RGBStringToIntArray(colors.Substring(j * 6, 6));
                    CudaProcessor.hCandidatesData[(i * cellStride) + (3 * j)] = color[0];
                    CudaProcessor.hCandidatesData[(i * cellStride) + (3 * j) + 1] = color[1];
                    CudaProcessor.hCandidatesData[(i * cellStride) + (3 * j) + 2] = color[2];
                }
            }

            //Setting up the solution data on the host
            for (int i = CudaProcessor.hSolutionData.Length - 1; i >= 0; i--)
            {
                CudaProcessor.hSolutionData[i] = 0;
            }
        }
    }
}
