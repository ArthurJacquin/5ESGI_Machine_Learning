#include <cstdlib>
#include <iostream>
#include <time.h>
#include <vector>
#include <exception>
#include "MLP.h"

/// <summary>
/// input = neurones en entrée du perceptron
/// sample = database à tester
/// </summary>

/// <summary>
/// sigmoid
/// </summary>
double Sign(double x)
{
	return 1 / (1 + exp(-x));
}

extern "C" {

#pragma region Linear
	//--------------------------------------Modèle linéaire-----------------------------------
	/// <summary>
	/// init des poids random
	/// </summary>
	__declspec(dllexport) double* create_linear_model(int inputs_count) {
		auto weights = new double[inputs_count + 1];
		srand(time(NULL));

		for (auto i = 0; i < inputs_count + 1; i++) {
			weights[i] = rand() / (double)RAND_MAX * 2.0 - 1.0;
		}

		return weights;
	}

	/// <summary>
	/// prédit un output 
	/// </summary>
	__declspec(dllexport) double predict_linear_model_classification(double* model, double samples[], int input_count) {
		
		double sum = model[0]; //poids du biais

		//somme de tout les samples * poids
		for (size_t i = 0; i < input_count; i++)
		{
			sum += samples[i] * model[i + 1];
		}

		//Sigmoid
		if(Sign(sum) < 0.5)
			return -1.0;
		
		return 1.0;
	}

	__declspec(dllexport) void train_linear_model_Rosenblatt(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int epochs, double learning_rate) {
		
		srand(time(NULL));

		//Repeter epochs fois
		for (size_t it = 0; it < epochs; it++)
		{
			int k = rand() % (sample_count); //Choix d'un indice random
			
			//Recup des inputs (X, Y)
			double* X = new double[input_count];
			for (size_t i = 0; i < input_count; i++)
				X[i] = all_samples[k * input_count + i];

			double Y = all_expected_outputs[k]; //Recup du resultat souhaité
			
			double p = predict_linear_model_classification(model, X, input_count); //Prediction du résultat
			
			//Mise à jour des poids
			model[0] = model[0] + learning_rate * (Y - p);
			for (size_t i = 0; i < input_count; i++)
			{
				//std::cout << "k : " << k << " | Y : " << Y << " | p : " << p << " | X[i] : " << X[i] << " = " << learning_rate * (Y - p) * X[i] << std::endl;

				model[i + 1] = model[i + 1] + learning_rate * (Y - p) * X[i];
			}
		}

		return;
	}

	__declspec(dllexport) void delete_model(double* model) {
		delete[] model;
	}
#pragma endregion

#pragma region MLP
	//--------------------------------------MLP-------------------------------------------------
	__declspec(dllexport) double* create_MLP_model(int dims[], int layer_count) {
		
		//Nombre total de poids
		int nbWeight = 0;
		for (size_t l = 1; l < layer_count; l++)
		{
			nbWeight += dims[l] * (dims[l - 1] + 1);
		}
		std::cout << "nbWeight "<<nbWeight << std::endl;
		
		//Init des poids en random
		double* w = new double[nbWeight];
		for (int i = 0; i < nbWeight; ++i)
		{
			w[i] = rand() / (double)RAND_MAX * 2.0 - 1.0;
		}
		return w;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="model"></param>
	/// <param name="inputs"></param>
	/// <param name="dimensions"> dimensions du réseau
	/// [0] -> inputs_count
	/// [1 .... N - 2] -> nb de neurones dans la couche
	/// [N - 1] -> nb outputs
	///  </param>
	/// <param name="isClassification">classification ou regression</param>
	__declspec(dllexport) double* predict_MLP(double* model,  double samples[], int* dims, int layer_count, bool isClassification)
	{
		MLP* mlp = new MLP(model, dims, layer_count);
		mlp->d = dims;
		mlp->w = model;
		mlp->L = layer_count - 1;

		for (int j = 0; j < mlp->d[0]; ++j)
		{
			mlp->x[j + 1] = samples[j];
		}

		int offsetW = 0;
		int offsetX = 0;
		for (int l = 1; l < mlp->L + 1; ++l) // Parcours des couches
		{
			if (l != 1)
			{
				offsetW += (mlp->d[l - 1]) * (mlp->d[l - 2] + 1);
			}
			offsetX += mlp->d[l - 1] + 1;

			for (int j = 1; j < mlp->d[l] + 1; ++j) // Parcours des neuronnes
			{
				double sum = 0.0;

				for (int i = 0; i < mlp->d[l - 1] + 1; ++i) // Parcours des poids
				{
					int idW = offsetW + (j - 1) + i * (mlp->d[l]);
					//std::cerr <<"offset =" << offsetX << " | j = " << j << " | i = " << i << "| idx =" << offsetX + j << std::endl;
					sum += mlp->x[offsetX - (mlp->d[l - 1] + 1) + i] * mlp->w[idW];
					//std::cerr << "idx =" << offsetX - (mlp->d[l - 1] + 1) + i << "X =" << mlp->x[offsetX + j] << " | W = " << mlp->w[idW] << std::endl;
				}
				if (l == mlp->L && !isClassification)
				{
					mlp->x[offsetX + j] = sum;
				}
				else
				{
					mlp->x[offsetX + j] = tanh(sum);
				}
			}
		}

		return mlp->x;
	}

	__declspec(dllexport) double* train_MLP(double* model, double allSamples[], int sampleCount, double allExpectedOutputs[], int* dims, int layer_count, bool isClassification, int epochs, double alpha)
	{
		MLP* mlp = new MLP(model, dims, layer_count);
		mlp->d = dims;
		mlp->w = model;
		mlp->L = layer_count - 1;

		//printArray(mlp->deltas, );

		int inputsSize = mlp->d[0];
		int outputsSize = mlp->d[mlp->L];

		srand(time(NULL));

		for (int it = 0; it < epochs; ++it)
		{
			int k = rand() % (sampleCount);

			double* x_k = new double[inputsSize];
			double* y_k = new double[outputsSize];


			//Init des inputs
			for (size_t i = 0; i < inputsSize; i++)
			{
				x_k[i] = allSamples[inputsSize * k + i];
			}

			for (size_t i = 0; i < outputsSize; i++)
			{
				y_k[i] = allExpectedOutputs[outputsSize * k + i];
			}

			mlp->x = predict_MLP(mlp->w, x_k, mlp->d, layer_count, isClassification);

			//Offset
			int offset = 0;
			for (size_t i = 0; i < mlp->L; i++)
			{
				offset += mlp->d[i];
			}
			offset += mlp->L;

			//Init des deltas
			for (int j = 1; j < mlp->d[mlp->L] + 1; ++j)
			{
				mlp->deltas[mlp->L][j] = mlp->x[offset + j] - y_k[j - 1];
				if (isClassification)
				{
					mlp->deltas[mlp->L][j] *= (1 - pow(mlp->x[offset + j], 2));
				}
			}

			//Retropropagation
			int offsetW = 0;
			for (int l = mlp->L; l > 1; --l) // Parcours des couches
			{
				if (l != 1)
				{
					offsetW += (mlp->d[l - 1]) * (mlp->d[l - 2] + 1);
				}

				//Offset X
				int offsetX = 0;
				for (size_t i = 0; i < l - 1; i++)
				{
					offsetX += mlp->d[i];
				}
				offsetX += l - 1;

				for (int i = 0; i < mlp->d[l - 1] + 1; ++i)
				{
					double sum = 0.0;

					for (int j = 1; j < mlp->d[l] + 1; ++j)
					{
						int idW = offsetW + (j - 1) + i * (mlp->d[l]);
						sum += mlp->w[idW] * mlp->deltas[l][j];
					}

					mlp->deltas[l - 1][i] = (1 - pow(mlp->x[offsetX + i], 2)) * sum;
				}
			}

			//Mise a jour des poids
			offsetW = 0;
			int offsetX = 0;
			for (int l = 1; l < mlp->L + 1; ++l)
			{
				if (l != 1)
				{
					offsetW += (mlp->d[l - 1]) * (mlp->d[l - 2] + 1);
					offsetX += mlp->d[l - 1];
				}

				for (int i = 0; i < mlp->d[l - 1] + 1; ++i)
				{
					for (int j = 1; j < mlp->d[l] + 1; ++j)
					{
						int id = offsetW + (j - 1) + i * (mlp->d[l]);
						mlp->w[id] -= alpha * mlp->x[offsetX + i] * mlp->deltas[l][j];
					}
				}
			}
		}

		return mlp->w;
	}
}
#pragma endregion