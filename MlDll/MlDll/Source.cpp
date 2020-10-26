#include <cstdlib>


extern "C" {
	__declspec(dllexport) double* create_linear_model(int inputs_count) {
		auto weights = new double[inputs_count + 1];
		for (auto i = 0; i < inputs_count + 1; i++) {
			weights[i] = rand() / (double)RAND_MAX * 2.0 - 1.0;
		}

		return weights;
	}

	__declspec(dllexport) double predict_linear_model_classification(double* model, double inputs[], int inputs_count) {
		// TODO
		return -1;
	}

	__declspec(dllexport) double* predict_linear_model_multiclass_classification(double* model, double inputs[], int inputs_count,
		int class_count) {
		// TODO
		return new double[3] {1.0, -1.0, 1.0};
	}

	__declspec(dllexport) void train_linear_model_rosenblatt(double* model, double all_inputs[], int inputs_count, int sample_count,
		double all_expected_outputs[], int expected_output_size, int epochs, double learning_rate) {
		// TODO
		return;
	}

	__declspec(dllexport) void delete_linear_model(double* model) {
		delete[] model;
	}

	__declspec(dllexport) double my_add(double a, double b) {
		return a + b + 2;
	}
}