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
/// sampleSize = nombres de data dans la database
/// dataSize = nombre de composante par data (1 pixel c'est 3 composantes x, y, z)
/// inputSize = nombre de pixels par image
/// </summary>

/// <summary>
/// sigmoid
/// </summary>

#pragma region DEBUG
	template<typename T>
	void print(T var, std::string name)
	{
		std::cout << name << " :\n" << var << std::endl;
	}

	void printVectorDouble(std::vector<double*> a, int datasize, std::string name)
	{
		std::cout << "-->" << name << std::endl;
		for (size_t i = 0; i < a.size(); i++)
		{
			std::cout << "(";
			for (size_t j = 0; j < datasize - 1; j++)
			{
				std::cout << a[i][j] << ", ";
			}

			std::cout << a[i][datasize - 1] << ")" << std::endl;
		}
	}

	void printArray(double* a, int size, int datasize, std::string name)
	{
		std::cout << "-->" << name << std::endl;
		for (size_t i = 0; i < size; i++)
		{
			std::cout << "(";
			for (size_t j = 0; j < datasize - 1; j++)
			{
				std::cout << a[i * datasize + j] << ", ";
			}

			std::cout << a[i * datasize + datasize - 1] << ")" << std::endl;
		}
	}
#pragma endregion

double Sign(double x)
{
	return 1 / (1 + exp(-x));
}

double* getOutput(double* outputTest, int outputSize, bool isClassification)
{
	if (isClassification)
	{
		//Recherche du max pour déterminer la classe
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

		printArray(outputTest, outputSize, 1, "outputs");
		//Si le maximum est trop petit -> appartient a aucune classe
		if (max < 0.6)
			return new double[1]{ -1.0 };

		return new double[1]{ double(id) };
	}
	else
	{
		return outputTest;
	}
}

extern "C" {

#pragma region Linear
	//--------------------------------------Modèle linéaire-----------------------------------
	/// <summary>
	/// init des poids random
	/// </summary>
	__declspec(dllexport) double* create_linear_model(int inputs_count, int outputCount) {
		double* weights = new double[inputs_count * outputCount + outputCount];
		srand(time(NULL));

		for (auto i = 0; i < inputs_count * outputCount + outputCount; i++) {
			weights[i] = rand() / (double)RAND_MAX * 2.0 - 1.0;
		}

		return weights;
	}

	//--------------------------------------Prédict------------------------------------------
	/// <summary>
	/// prédit un output 
	/// </summary>
	__declspec(dllexport) double* predict_linear_model(double* model, double samples[], int input_count, int outputSize, bool isClassification) 
	{
		double* outputTest = new double[outputSize];

		//somme de tout les samples * poids
		for (size_t i = 0; i < outputSize; i++)
		{
			double sum = model[i]; //poids du biais

			//somme de tout les samples * poids
			for (size_t j = 0; j < input_count; j++)
			{
				sum += samples[j] * model[j * outputSize + i + outputSize];
			}

			if (isClassification)
			{
				if(Sign(sum) < 0.5)
					outputTest[i] = 0.0;
				else
					outputTest[i] = 1.0;
			}
			else
			{
				outputTest[i] = sum;
			}
		}

		return getOutput(outputTest, outputSize, isClassification);
	}
	
	double* predictForTraining(double* model, double* samples, int input_count, int outputSize)
	{
		double* outputTest = new double[outputSize];

		for (size_t i = 0; i < outputSize; i++)
		{
			double sum = model[i]; //poids du biais

			//somme de tout les samples * poids
			for (size_t j = 0; j < input_count; j++)
			{
				sum += samples[j] * model[j * outputSize + i + outputSize];
			}

			if (Sign(sum) < 0.5)
				outputTest[i] = 0.0;
			else
				outputTest[i] = 1.0;
		}

		return outputTest;
	}

	//---------------------------------------Train----------------------------------------------
	void train_linear_model_classification(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int outputSize, int epochs, double learning_rate)
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

			double* Y = new double[outputSize];
			for (size_t i = 0; i < outputSize; i++)
			{
				Y[i] = all_expected_outputs[k * outputSize + i]; //Recup du resultat souhaité
			} 

			double* p = predictForTraining(model, X, input_count, outputSize); //Prediction du résultat

			for (size_t o = 0; o < outputSize; o++)
			{
				//Mise à jour des poids
				double error = Y[o] - p[o];
				model[o] += learning_rate * error;
				for (size_t i = 0; i < input_count; i++)
				{
					model[i * outputSize + o + outputSize] += learning_rate * error * X[i];
				}
			}
		}
	}

	//TODO : A debug
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

		//Matrices avec les outputs attendus
		MatrixXd Y(sample_count, 1);
		for (size_t i = 0; i < sample_count; i++)
		{
			Y(i, 0) = all_expected_outputs[i];
		}

		//Matrices pour la mise a jour des poids
		MatrixXd m = ((X.transpose() * X).inverse() * X.transpose()) * Y;

		//Mise a jour des poids
		//std::cout << "model : " << std::endl;
		for (size_t i = 0; i < input_count + 1; i++)
		{
			//std::cout << model[i] << std::endl;
			model[i] /= m(i, 0);
		}
	}

	__declspec(dllexport) void train_linear_model(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int outputSize, int epochs, double learning_rate, bool isClassification) 
	{

		if (isClassification)
			train_linear_model_classification(model, all_samples, sample_count, input_count, all_expected_outputs, outputSize, epochs, learning_rate);
		else
			train_linear_model_regression(model, all_samples, sample_count, input_count, all_expected_outputs, epochs, learning_rate);
	}

	//-------------------------------------Delete---------------------------------------------
	__declspec(dllexport) void delete_model(double* model) 
	{
		delete[] model;
	}
#pragma endregion

#pragma region MLP
	//--------------------------------------MLP-------------------------------------------------
	__declspec(dllexport) double* create_MLP_model(int dims[], int layer_count) 
	{
		//Nombre total de poids
		int nbWeight = 0;
		for (size_t l = 1; l < layer_count; l++)
		{
			nbWeight += dims[l] * (dims[l - 1] + 1);
		}
		std::cout << "nbWeight " << nbWeight << std::endl;

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
	/// <param name="dimensions"> dimensions du réseau
	/// [0] -> inputs_count
	/// [1 .... N - 2] -> nb de neurones dans la couche
	/// [N - 1] -> nb outputs
	///  </param>
	/// <param name="isClassification">classification ou regression</param>
	__declspec(dllexport) double* predict_MLP(double* model, double samples[], int* dims, int layer_count, bool isClassification)
	{
		std::cout << "---------------PREDICT--------------------" << std::endl;
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

		//Recupération des outputs
		double* outputs = new double[dims[layer_count - 1]];
		int idLastLayer = mlp->node_count - mlp->d[mlp->L];
		for (size_t i = idLastLayer; i < mlp->node_count; i++)
		{
			outputs[i - idLastLayer] = mlp->x[i];
		}

		return getOutput(outputs, mlp->d[mlp->L], isClassification);
	}

	double* predictMLPForTraining(double* model, double samples[], int* dims, int layer_count, bool isClassification)
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
		std::cout << "---------------TRAINING--------------------" << std::endl;
		MLP* mlp = new MLP(model, dims, layer_count);
		mlp->d = dims;
		mlp->w = model;
		mlp->L = layer_count - 1;

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

			mlp->x = predictMLPForTraining(mlp->w, x_k, mlp->d, layer_count, isClassification);

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
			if (dims[layer_count - 1] == 1)
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
#pragma endregion

#pragma region RBF
	//--------------------------------------RBF-------------------------------------------------

	/// <summary>
	/// Parse les informations dans samples pour facilité l'utilisation
	/// </summary>
	void parseSample(std::vector<double*>& data, double* samples, int sampleSize, int inputSize, int dataSize)
	{
		data.resize(sampleSize);
		//boucle sur tous les indices de samples
		for (size_t i = 0; i < sampleSize; i++)
		{
			//boucle sur tous les pixels de l'image
			data[i] = new double[inputSize * dataSize];
			for (size_t j = 0; j < inputSize * dataSize; j++)
			{
				data[i][j] = samples[i * inputSize * dataSize + j];
			}
		}
	}

	/// <summary>
	/// Moyenne des pixels d'une image pour retourner un centre
	/// </summary>
	double* calculateCenter(double* X, int inputSize, int dataSize)
	{
		double* center = new double[dataSize];

		//Init center
		for (size_t j = 0; j < dataSize; j++)
		{
			center[j] = 0.0;
		}

		for (size_t i = 0; i < inputSize; i += dataSize)
		{
			for (size_t j = 0; j < dataSize; j++)
			{
				center[j] += X[i * dataSize + j];
			}
		}

		for (size_t j = 0; j < dataSize; j++)
		{
			center[j] = center[j] / inputSize;
		}

		return center;

	}

	/// <summary>
	/// Fonction gaussienne d'activation => distance entre les centres de l'image center1 et center2 image de test
	/// En entrée : deux centre d'images center1 et center2
	/// </summary>
	double gaussianFunction(double* center1, double* center2, double gamma, int dataSize)
	{
		double distance = 0.0;

		//calcul de distance
		for (size_t i = 0; i < dataSize; i++)
		{
			distance += pow((center1[i] - center2[i]), 2.0);
		}

		return exp(-gamma * distance);
	}

	/// <summary>
	/// Calcule la distance entre 2 centres
	/// </summary>
	float distance(double* x1, double* x2, int dataSize)
	{
		float distance = 0.0;
		for (size_t i = 0; i < dataSize; i++)
		{
			distance += pow(x1[i] - x2[i], 2.0);
		}

		return sqrt(distance);
	}

	//----------------------------------TRAINING FUNCTIONS--------------------------------------------
	/// <summary>
	/// Retourne tableau avec les centres
	/// K : nombre de clusters voulus
	/// </summary>
	std::vector<double*> lloydAlgorithm(std::vector<double*>& samples, int K, int sampleSize, int inputSize, int dataSize)
	{
		std::vector<double*> centers;

		//initialisation random des centers
		for (size_t i = 0; i < K; i++)
		{
			centers.push_back(calculateCenter(samples[i], inputSize, dataSize));
			//centers.push_back(calculateCenter(samples[rand() % (sampleSize)], inputSize, dataSize));
		}

		std::vector<double*> dataSumPerCenter; //Somme de tout les samples dans chaque cluster
		std::vector<int> datasPerCenter; //Nombre de samples dans chaque cluster

		//Initialisation des tableaux
		dataSumPerCenter.resize(K);
		datasPerCenter.resize(K);
		for (size_t i = 0; i < K; ++i)
		{
			dataSumPerCenter[i] = new double[dataSize];
			for (size_t j = 0; j < dataSize; ++j)
			{
				dataSumPerCenter[i][j] = 0.0;
			}

			datasPerCenter[i] = 0;
		}

		//assignation des data aux clusters les plus proches
		//boucle sur les datas
		for (size_t d = 0; d < sampleSize; d++)
		{
			double* currentCenter = calculateCenter(samples[d], inputSize, dataSize);
			double min = distance(centers[0], currentCenter, dataSize);
			int minIndex = 0;

			//boucle sur les clusters
			for (size_t c = 1; c < K; c++)
			{
				double dist = distance(centers[c], currentCenter, dataSize);
				if (dist < min)
				{
					min = dist;
					minIndex = c;
				}
			}

			//ajout data dans le cluster le plus proche 
			for (size_t i = 0; i < dataSize; i++)
			{
				//std::cout << "minIndex : " << minIndex << " | dataSumPerCenter : " << dataSumPerCenter.size()  << " | i : " << i << std::endl;
				dataSumPerCenter[minIndex][i] += currentCenter[i];
			}

			//une data en plus dans le cluster
			datasPerCenter[minIndex] += 1;
		}

		//réassignation : moyenne des centres 
		for (size_t i = 0; i < K; i++)
		{
			for (size_t j = 0; j < dataSize; j++)
			{
				centers[i][j] = dataSumPerCenter[i][j] / datasPerCenter[i];
			}
		}

 		return centers;
	}

	/// <summary>
	/// Calcule les points en fonctions des centres précalculés et des outputs souhaités
	/// </summary>
	void calculateWeight(double*& model, int K, int sampleSize, std::vector<double*> lloydCenters, std::vector<double*>& samples, int inputSize, int dataSize, double gamma, double* output, int outputCount)
	{
		MatrixXd X(sampleSize, K);

		for (size_t i = 0; i < X.rows(); i++)
		{
			for (size_t j = 0; j < X.cols(); j++)
			{
				double* center1 = calculateCenter(samples[i], inputSize, dataSize);
				X(i, j) = gaussianFunction(center1, lloydCenters[j], gamma, dataSize);
			}
		}

		MatrixXd outputMat(sampleSize, outputCount);

		for (size_t i = 0; i < outputMat.rows(); i++)
		{
			for (size_t j = 0; j < outputMat.cols(); j++)
			{
				outputMat(i, j) = output[i * outputCount + j];
			}
		}

		MatrixXd result = (X.transpose() * X).inverse() * X.transpose() * outputMat;

		for (size_t i = 0; i < result.rows(); i++)
		{
			for (size_t j = 0; j < result.cols(); j++)
			{
				model[i * result.cols() + j] = result(i, j);
			}
		}
	}

	//--------------------------------------EXPORTED FUNCTIONS-------------------------------------------------

	//layer count = 1
	//1 ere partie de model : w 
	//2 eme partie de model : centers
	__declspec(dllexport) double* create_RBF_model(int dims[], int dataSize)
	{
		std::cout << "---------------Model creation--------------------" << std::endl;
		int modelSize = (dims[0] * dims[1]) + (dims[0] * dataSize);
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

	/// <summary>
	/// training 
	/// </summary>
	__declspec(dllexport) double* training_RBF_model(double* model, int dims[], double samples[], int sampleSize, int inputSize, int dataSize, double output[], int epoch, double gamma)
	{
		std::cout << "---------------TRAINING--------------------" << std::endl;
		
		std::cout << "---------------Parsing--------------------" << std::endl;
		//Parse samples
		std::vector<double*> data;
		parseSample(data, samples, sampleSize, inputSize, dataSize);

		int K = dims[0];
		int outputCount = dims[1];

		std::cout << "---------------Lloyd algo--------------------" << std::endl;
		//initialize center
		std::vector<double*> lloydCenters = lloydAlgorithm(data, K, sampleSize, inputSize, dataSize);
		//printVectorDouble(lloydCenters, dataSize, "centers");

		std::cout << "---------------Weights Update--------------------" << std::endl;
		//calcule les poids 
		for (size_t i = 0; i < epoch; i++)
		{
			calculateWeight(model, K, sampleSize, lloydCenters, data, inputSize, dataSize, gamma, output, outputCount);
		}
		//printArray(model, K, outputCount, "Weights");

		//concatene les centres dans model
		for (size_t i = 0; i < K; i++)
		{
			for (size_t j = 0; j < dataSize; j++)
			{
				model[K * outputCount + i * dataSize + j] = lloydCenters[i][j];
			}
		}

		return model;
	}

	/// <summary>
	/// predict 
	/// </summary>
	__declspec(dllexport) double* predict_RBF_model(double* model, int dims[], double samples[], int inputSize, int dataSize, bool isClassification, double gamma)
	{
		std::cout << "---------------PREDICT--------------------" << std::endl;
		int outputSize = dims[1];
		RBF* rbf = new RBF(dims, dataSize);
		//init w
		for (size_t i = 0; i < rbf->wSize; i++)
		{
			rbf->w[i] = model[i];
		}
		//printArray(rbf->w, rbf->wSize, 1, "result");

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

		return getOutput(outputTest, outputSize, isClassification);
	}

#pragma endregion
}