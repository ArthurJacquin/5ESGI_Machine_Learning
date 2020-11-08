#include <cstdlib>
#include <iostream>
#include <time.h>
#include <vector>
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

	//--------------------------------------MLP-------------------------------------------------
	__declspec(dllexport) double* create_MLP_model(int dims[], int layer_count) {
		
		//Nombre total de poids
		int nbWeight = 0;
		for (size_t l = 1; l < layer_count + 1; l++)
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
	__declspec(dllexport) double predict_MLP(double* model,  double samples[], int* dimensions, int layer_count, bool isClassification)
	{
		MLP* mlp = new MLP(model, dimensions, layer_count);
		mlp->d = dimensions;
		mlp->w = model;
		mlp->L = layer_count - 1;

		for (int j = 0; j < mlp->d[0]; ++j)
		{
			mlp->x[0][j + 1] = samples[j];
		}

		int offset = 0;
		for (int l = 1; l < mlp->L + 1; ++l) //Parcours des couches
		{
			if (l != 1)
				offset += (mlp->d[l - 1]) * (mlp->d[l - 2] + 1);

			for (int j = 1; j < mlp->d[l] + 1; ++j) //Parcours des neuronnes
			{
				double sum = 0.0;

				for (int i = 0; i < mlp->d[l - 1] + 1; ++i) //Parcours des poids
				{
					int id = offset + (j - 1) + i * (mlp->d[l]);
					//std::cerr <<"offset =" << offset << " | j = " << j << " | i = " << i << " | id = " << id << std::endl;
					sum += mlp->x[l - 1][i] * mlp->w[id];
					std::cerr << "x =" << mlp->x[l - 1][i] << " | w = " << mlp->w[id] << std::endl;
				}

				if (l == mlp->L && !isClassification)
				{
					mlp->x[l][j] = sum;
				}
				else
				{
					mlp->x[l][j] = tanh(sum);
				}
			}
		}

		return mlp->x[mlp->L][1];
	}

	__declspec(dllexport) void train_MLP(MLP* model, double allInputs[], double allExpectedOutputs[],
		bool isClassification, int sampleCount, int epochs, double alpha)
	{
		int inputsSize = model->d[0];
		int outputsSize = model->d[model->L];

		for (int it = 0; it < epochs; ++it)
		{
			int k = rand() % sampleCount - 1 + 0;
		}
	}
}