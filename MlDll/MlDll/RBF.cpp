#include "RBF.h"

RBF::RBF(int dims[], int dataSize)
{
	//couche gaussienne + output
	d = new int[2];

	wSize = dims[0] * dims[1];
	w = new double[wSize];

	cSize = dims[0] * dataSize;
	c = new double[cSize];
}


RBF::~RBF()
{
	delete[] d;
	delete[] w;
	delete[] c;
}