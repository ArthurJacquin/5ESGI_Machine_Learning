#include "MLP.h"
#include <iostream>

//Auteurs : Benoit, Arthur et Margot
MLP::MLP(double* weights, int dims[], int layer_count)
{
	d = new int[layer_count];
	node_count = 0;
	for (int i = 0; i < layer_count; ++i)
	{
		d[i] = dims[i];
		node_count += d[i];
	}
	node_count += layer_count;

	L = layer_count - 1;

	x = new double[node_count];
	int offset = 0;
	for (int l = 0; l < layer_count; ++l)
	{
		if (l != 0)
			offset += d[l - 1] + 1;

		for (int j = 0; j < dims[l] + 1; ++j)
		{
			if (j > 0)
			{
				x[offset + j] = 0.0;
			}
			else
			{
				x[offset + j] = 1.0;
			}
		}
	}

	deltas.resize(layer_count);
	for (int l = 0; l < layer_count; ++l)
	{
		for (int j = 0; j < dims[l] + 1; ++j)
		{
			if (j > 0)
			{
				deltas[l].push_back(0.0);
			}
			else
			{
				deltas[l].push_back(1.0);
			}
		}
	}
}

MLP::~MLP()
{
	delete[] d;
	delete[] w;
}