#include <cstdlib>
#include <iostream>
#include <time.h>
#include <vector>
#include <exception>
#include <Eigen/Dense>
#include "MLP.h"
#include "RBF.h"
#include "../TestMLDLL/Test.h"

using namespace Eigen;

/// <summary>
/// input = neurones en entrée du perceptron
/// sample = database à tester
/// dataSize = nombre de composante par data (1 pixel c'est 3 composantes x, y, z)
/// inputSize = nombre de pixels par image
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

	//--------------------------------------Prédict------------------------------------------
	/// <summary>
	/// prédit un output 
	/// </summary>
	__declspec(dllexport) double predict_linear_model(double* model, double samples[], int input_count, bool isClassification) {
		
		double sum = model[0]; //poids du biais

		//somme de tout les samples * poids
		for (size_t i = 0; i < input_count; i++)
		{
			sum += samples[i] * model[i + 1];
		}

		if (isClassification)
		{
			if (Sign(sum) < 0.5)
				return -1.0;
			else
				return 1.0;
		}
		else
		{
			return sum;
		}
	}

	//---------------------------------------Train----------------------------------------------
	void train_linear_model_classification(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int epochs, double learning_rate)
	{
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

			double p = predict_linear_model(model, X, input_count, true); //Prediction du résultat

			//Mise à jour des poids
			model[0] = model[0] + learning_rate * (Y - p);
			for (size_t i = 0; i < input_count; i++)
			{
				model[i + 1] = model[i + 1] + learning_rate * (Y - p) * X[i];
			}
		}
	}

	void train_linear_model_regression(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int epochs, double learning_rate)
	{
		//Matrices avec tout les samples + les biais
		MatrixXd X(sample_count, input_count + 1);
		for (size_t i = 0; i < sample_count; i++)
		{
			X(i, 0) = 1.0;

			for (size_t j = 0; j < input_count; j++)
			{
				X(i, j + 1) = all_samples[i * input_count + j];
			}
		}
		std::cout << "X : " << std::endl << X << std::endl;

		//Matrices avec les outputs attendus
		MatrixXd Y(sample_count, 1);
		for (size_t i = 0; i < sample_count; i++)
		{
			Y(i, 0) = all_expected_outputs[i];
		}
		std::cout << "Y : " << std::endl << Y << std::endl;

		//Matrices pour la mise a jour des poids
		MatrixXd m = ((X.transpose() * X).inverse() * X.transpose()) * Y;
		std::cout << "trans : " << std::endl << X.transpose() << std::endl;
		std::cout << "mul : " << std::endl << X.transpose() * X << std::endl;
		std::cout << "inverse : " << std::endl << (X.transpose() * X).inverse() << std::endl;
		std::cout << "M : " << std::endl << m << std::endl;

		//Mise a jour des poids
		//std::cout << "model : " << std::endl;
		for (size_t i = 0; i < input_count + 1; i++)
		{
			//std::cout << model[i] << std::endl;
			model[i] /= m(i, 0);
		}
	}

	__declspec(dllexport) void train_linear_model(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int epochs, double learning_rate, bool isClassification) {
		
		if (isClassification)
			train_linear_model_classification(model, all_samples, sample_count, input_count, all_expected_outputs, epochs, learning_rate);
		else
			train_linear_model_regression(model, all_samples, sample_count, input_count, all_expected_outputs, epochs, learning_rate);
	}

	//-------------------------------------Delete---------------------------------------------
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

	_declspec(dllexport) double* export_result(int sample_count, double* samples, double* outputs, int layer_count, int* dims, int node_count, bool isClassification, int epoch, double alpha)
	{
		double* model = create_MLP_model(dims, layer_count);

		//Training
		train_MLP(model, samples, sample_count, outputs, dims, layer_count, isClassification, epoch, alpha);

		//Enregistrement des résultats prédits
		double* result = new double[sample_count];
		for (size_t i = 0; i < sample_count; i++)
		{
			double* result_tmp = predict_MLP(model, new double[2]{ samples[i * 2], samples[i * 2 + 1] }, dims, layer_count, isClassification);
			if(dims[layer_count - 1] == 1)
			{
				result[i] = result_tmp[node_count - 1];
			}
			else //multi class 
			{
				//TODO : faire des trucs
				result[i] = result_tmp[node_count - 1];
			}
			
		}

		delete_model(model);
		
		return result;
	}
}
#pragma endregion

#pragma region RBF
//--------------------------------------RBF-------------------------------------------------
//moyenne des pixels d'une image pour retourner un centre (lloyd algo)
double* calculateCenter(double* X, int inputSize, int dataSize)
{
	double* center = new double[dataSize];

	for (size_t i = 0; i < inputSize; i+= dataSize)
	{
		for (size_t j = 0; j < dataSize; j ++)
		{
			center[j] += X[j + i];
		}
	}

	for (size_t j = 0; j < dataSize; j++)
	{
		center[j] = center[j] / inputSize;
	}

	return center;

}

//fonction gaussienne d'activation => distance entre les centres de l'image center1 et center2 image de test
//En entrée : deux centre d'images center1 et center2
double gaussianFunction(double* center1, double* center2, double gamma, int dataSize)
{
	double distance = 0.0;

	//calcul de distance
	for (size_t i = 0; i < dataSize; i++)
	{
		distance += pow((center1[i] - center2[i]), 2.0);
	}

	return exp( - gamma * distance );
}
//-------------------------------------------Model-----------------------------------------
//layer count = 1
//1 ere partie de model : w 
//2 eme partie de model : centers
__declspec(dllexport) double* create_RBF_model(int dims[], int dataSize)
{
	int modelSize = ( dims[0] * dims[1] ) + ( dims[0] * dataSize );
	double* model = new double[modelSize];

	//Init des poids en random
	int nbWeight = dims[0] * dims[1];

	for (int i = 0; i < nbWeight; ++i)
	{
		model[i] = rand() / (double)RAND_MAX * 2.0 - 1.0;
	}

	for (int i = nbWeight; i < modelSize; i++)
		model[i] = 0.0;

	return model;
}

//--------------------------------------Prédict------------------------------------------
double loydAlgorithm()
{

}

/// <summary>
/// predict 
/// </summary>
__declspec(dllexport) double predict_RBF_model(double* model, int dims[], int dataSize, double samples[], int inputSize, int outputSize, bool isClassification, float gamma) 
{
	RBF* rbf = new RBF(dims, dataSize);

	//init w
	for (size_t i = 0; i < rbf->wSize; i++)
	{
		rbf->w[i] = model[i];
	}

	//init c
	for (size_t i = 0; i < rbf->cSize; i++)
	{
		rbf->c[i] = model[i + rbf->wSize];
	}

	double* center1 = calculateCenter(samples, inputSize, dataSize);
	double* center2 = new double[dataSize];

	double* outputTest = new double[outputSize];

	//somme de tout les centres * poids pour un output
	//boucle sur les outputs
	for (size_t k = 0; k < outputSize; k++)
	{
		double sum = 0.0;
		//boucle sur les centres
		for (size_t i = 0; i < dims[0]; i++)
		{
			//boucle sur les composantes de chaque centre (r, g, b)
			for (size_t j = 0; j < dataSize; j++)
			{
				center2[j] = rbf->c[i * dataSize + j];
			}

			sum += gaussianFunction(center1, center2, gamma, dataSize) * rbf->w[i * outputSize + k];
		}

		if (isClassification)
		{
			outputTest[k] = Sign(sum);
		}
		else
		{
			outputTest[k] = sum;
		}
	}

	double max = 0.;
	int id = 0;
	for (size_t i = 0; i < outputSize; i++)
	{
		if (outputTest[i] > max)
		{
			max = outputTest[i];
			id = i;
		}
	}

	return id;
}


#pragma endregion