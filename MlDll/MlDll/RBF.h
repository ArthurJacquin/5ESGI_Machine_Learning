#pragma once
#include <vector>

struct RBF
{
	//d : nbre input + cluster + output
	int* d;
	//w : tableau de poids
	double* w;
	int wSize;
	//c : tableau de centre des clusters
	double* c;
	int cSize;

	RBF(int dims[], int dataSize);
	~RBF();
};