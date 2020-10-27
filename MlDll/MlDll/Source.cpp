#include <cstdlib>
#include <iostream>
#include <time.h>

double Sign(double x)
{
	return 1 / (1 + exp(-x));
}

extern "C" {

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

	__declspec(dllexport) double* predict_linear_model_multiclass_classification(double* model, double samples[], int sample_count,
		int class_count) {
		// TODO
		return new double[3] {1.0, -1.0, 1.0};
	}

	__declspec(dllexport) void train_linear_model_Rosenblatt(double* model, double all_samples[], int sample_count, int input_count,
		double all_expected_outputs[], int epochs, double learning_rate) {
		
		srand(time(NULL));

		//Repeter N fois
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
			for (size_t i = 0; i < input_count; i++)
			{
				if(it > 9990)
					std::cout << "k : " << k << " | Y : " << Y << " | p : " << p << " | X[i] : " << X[i] << " = " << learning_rate * (Y - p) * X[i] << std::endl;

				model[i + 1] += learning_rate * (Y - p) * X[i];
			}
		}

		return;
	}

	__declspec(dllexport) void delete_linear_model(double* model) {
		delete[] model;
	}
}