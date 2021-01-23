#pragma once
#include <vector>

struct MLP
{
	int* d; //Dimensions
	int L; //Nombre de couches
	double* w; //Poids
	double* x; //Valeurs des neuronnes
	std::vector<std::vector<double>> deltas;

	int node_count;

	MLP(double* weights, int dims[], int layer_count);
	~MLP();
};