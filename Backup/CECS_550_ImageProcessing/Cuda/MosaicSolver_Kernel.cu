// MosaicSolver_Kernel.cu

/* Matrix multiplication: C = A * B.
 * Device code.
 */

#ifndef _MOSAICSOLVER_KERNEL_H_
#define _MOSAICSOLVER_KERNEL_H_

#include <stdio.h>

#define BLOCK_SIZE 16

#define CHECK_BANK_CONFLICTS 0
#if CHECK_BANK_CONFLICTS
#define MasterS(i, j) cutilBankChecker(((int*)&Master_s[0][0]), (BLOCK_SIZE * i + j))
#define CandidatesS(i, j, k) cutilBankChecker(((int*)&Candidates_s[0][0][0]), (BLOCK_SIZE * (BLOCK_SIZE * i + j)) + k)
#else
#define MasterS(i, j) Master_s[i][j]
#define CandidatesS(i, j, k) Candidates_s[i][j][k]
#endif

extern "C" __global__ void
solveMosaic( int* Solution, int* Master, int* Candidates, int subdivLevel, int numCandidates, int miWidthInCells)
{
	int cellY = (blockIdx.y * BLOCK_SIZE) + threadIdx.y;
	int cellX = (blockIdx.x * BLOCK_SIZE) + threadIdx.x;
	
	int cellRowWidth = 3 * subdivLevel;			// the 3 is for R,G,B
	int cellStride = 3 * subdivLevel * subdivLevel;
	
	int masterR, masterG, masterB;
	int candidateR, candidateG, candidateB;
	
	int baseIndex = (cellStride * (miWidthInCells * cellY)) + cellX * cellRowWidth;
	int slnBaseIndex = numCandidates * ((miWidthInCells * cellY) + cellX);
	for(int x = 0; x < subdivLevel; x++){
		for(int y = 0; y < subdivLevel; y++){
		
			//Getting the master image colors
			masterR = Master[baseIndex + (cellRowWidth * (miWidthInCells * y)) + (x * 3)];
			masterG = Master[baseIndex + (cellRowWidth * (miWidthInCells * y)) + (x * 3) + 1];
			masterB = Master[baseIndex + (cellRowWidth * (miWidthInCells * y)) + (x * 3) + 2];
			
			for(int n = 0; n < numCandidates; n++){
			
				//Getting the candidate image color
				candidateR = Candidates[(cellStride * n) + (cellRowWidth * y) + (3 * x)];
				candidateG = Candidates[(cellStride * n) + (cellRowWidth * y) + (3 * x) + 1];
				candidateB = Candidates[(cellStride * n) + (cellRowWidth * y) + (3 * x) + 2];
				
				//Updating the manhattan style distance between the cell and candidate image
				Solution[slnBaseIndex + n] += (abs(masterR - candidateR) + abs(masterG - candidateG) + abs(masterB - candidateB));
			}
		}
	}
}

#endif // #ifndef _MOSAICSOLVER_KERNEL_H_
