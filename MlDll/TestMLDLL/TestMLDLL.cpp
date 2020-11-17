#include <iostream>
#include "Test.h"

extern "C"
{
    //------------------------Modèle linéaire----------------------------------------
    __declspec(dllimport) double* create_linear_model(int inputs_count);
    __declspec(dllimport) double predict_linear_model_classification(double* model, double inputs[], int sample_count);
    __declspec(dllimport) void train_linear_model(double* model, double all_samples[], int sample_count, int input_count,
        double all_expected_outputs[], int epochs, double learning_rate, bool isClassification);
    __declspec(dllimport) void delete_model(double* model);

    //------------------------MLP------------------------------------------------------
    __declspec(dllimport) double* create_MLP_model(int dims[], int layer_count);
    __declspec(dllimport) double* predict_MLP(double* model, double samples[], int* dimensions, int layer_count, bool isClassification);
    __declspec(dllimport) double* train_MLP(double* model, double allSamples[], int sampleCount, double allExpectedOutputs[],
        int* dims, int layer_count, bool isClassification, int epochs, double alpha);
	__declspec(dllimport) double* export_result(int type, int layer_count, int* dims,
        int node_count, bool isClassification, int epoch, double alpha);
}

int main()
{
    std::cout.precision(5);
    getchar();

    CasTest test(TestType::LinearSimple2D);
    test.DisplayInfos();

    //Variables
    int layer_count = 3;
    int* dims = new int[layer_count] { 2, 3, 1 };
    int epoch = 1000;
    double alpha = 0.1;
    bool isClassification = false;

    int node_count = 0;
    for (int i = 0; i < layer_count; ++i)
    {
        node_count += dims[i];
    }
    node_count += layer_count;

    //Creation du model
    double* model = create_MLP_model(dims, layer_count);

    //Linear
#if 1
    std::cout << "BEFORE !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i ++)
    {
        std::cout << predict_linear_model_classification(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, 2) << std::endl;
    }

    train_linear_model(model, test.samples, test.sample_count, 2, test.outputs, epoch, alpha, isClassification);

    std::cout << "AFTER !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i++)
    {
        std::cout << predict_linear_model_classification(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, 2) << std::endl;
    }
#endif

    //MLP
#if 0
    std::cout << "BEFORE !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i++)
    {
        double* result = predict_MLP(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, dims, layer_count, isClassification);
        std::cout << " resultat : " << result[node_count - 1] << std::endl;
    }

    train_MLP(model, test.samples, test.sample_count, test.outputs, dims, layer_count, isClassification, epoch, alpha);

    std::cout << "AFTER !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i++)
    {
        double* result = predict_MLP(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, dims, layer_count, isClassification);
        std::cout << " resultat : " << result[node_count - 1] << std::endl;
    }

#endif 

    delete_model(model);

   /*int layer_count = 3;
    int* dims = new int[layer_count] { 2, 3, 1 };
    int node_count = 0;
    for (int i = 0; i < layer_count; ++i)
    {
        node_count += dims[i];
    }
    node_count += layer_count;

	double* exportResults = export_result(2, layer_count, dims, node_count, true, 1000, 0.1);

	for (size_t i = 0; i < 4; i++)
    {
        std::cout << " resultat : " << exportResults[i] << std::endl;
    }
	return 0;*/
}