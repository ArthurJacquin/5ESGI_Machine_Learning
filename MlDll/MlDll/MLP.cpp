#include "MLP.h"

MLP::MLP(double* weights, int dims[], int layer_count)
{
	d = new int[layer_count];
	for (int i = 0; i < layer_count; ++i)
	{
		d[i] = dims[i];
	}

	L = layer_count - 1;

	x.resize(layer_count);
	for (int l = 0; l < layer_count; ++l)
	{
		for (int j = 0; j < dims[l] + 1; ++j)
		{
			if (j > 0)
			{
				x[l].push_back(0.0);
			}
			else
			{
				x[l].push_back(1.0);
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
				deltas[l].emplace_back(0.0);
			}
			else
			{
				deltas[l].emplace_back(1.0);
			}
		}
	}
}

MLP::~MLP()
{
	delete[] d;
	delete[] w;
}