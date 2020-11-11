#pragma once
#include <vector>

struct MLP
{
	int* d;
	int L;
	double* w;
	double* x;
	std::vector<std::vector<double>> deltas;

	int node_count;

	MLP(double* weights, int dims[], int layer_count);
	~MLP();
};