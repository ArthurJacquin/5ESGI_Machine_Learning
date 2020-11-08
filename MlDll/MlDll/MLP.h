#pragma once
#include <vector>

struct MLP
{
	int* d;
	int L;
	double* w;
	std::vector<std::vector<double>> x;
	std::vector<std::vector<double>> deltas;

	MLP(double* weights, int dims[], int layer_count);
	~MLP();
};