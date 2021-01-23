#include <iostream>
#include "Test.h"

extern "C"
{
    //------------------------Modèle linéaire----------------------------------------
    __declspec(dllimport) double* create_linear_model(int inputs_count, int outputCount);
    __declspec(dllimport) double predict_linear_model(double* model, double samples[], int input_count, int outputCount, bool isClassification);
    __declspec(dllimport) void train_linear_model(double* model, double all_samples[], int sample_count, int input_count,
        double all_expected_outputs[], int outputCount, int epochs, double learning_rate, bool isClassification);
    __declspec(dllimport) void delete_model(double* model);

    //------------------------MLP------------------------------------------------------
    __declspec(dllimport) double* create_MLP_model(int dims[], int layer_count);
    __declspec(dllimport) double* predict_MLP(double* model, double samples[], int* dimensions, int layer_count, bool isClassification);
    __declspec(dllimport) double* train_MLP(double* model, double allSamples[], int sampleCount, double allExpectedOutputs[],
        int* dims, int layer_count, bool isClassification, int epochs, double alpha);
	__declspec(dllimport) double* export_result(int type, int layer_count, int* dims,
        int node_count, bool isClassification, int epoch, double alpha);

    //------------------------RBF------------------------------------------------------
    __declspec(dllimport) double* create_RBF_model(int dims[], int dataSize);
    __declspec(dllimport) double* training_RBF_model(double* model, int dims[], double* samples, int sampleSize, int inputSize, int dataSize, double* output, int epoch, double gamma);
    __declspec(dllimport) double* predict_RBF_model(double* model, int dims[], double* samples, int inputSize, int dataSize, bool isClassification, float gamma);
}

int main()
{
    std::cout.precision(5);
    getchar();

    CasTest test(TestType::LinearMultipleMulticlass);
    
    test.DisplayInfos();

    //Variables
    int epoch = 100;
    double alpha = 0.1;
    bool isClassification = true;

    //-------------------------------------Linear-----------------------------------------
#if 1
    //Creation du model
    int input_count = 2;
    int outputSize = 2;
    double* model = create_linear_model(input_count, outputSize);

    std::cout << "BEFORE !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i ++)
    {
        std::cout << predict_linear_model(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, input_count, outputSize, isClassification) << std::endl;
    }

    train_linear_model(model, test.samples, test.sample_count, input_count, test.outputs, outputSize, epoch, alpha, isClassification);

    std::cout << "AFTER !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i++)
    {
        std::cout << predict_linear_model(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, input_count, outputSize, isClassification) << std::endl;
    }
#endif

    //-------------------------------------MLP-----------------------------------------
#if 0
    int layer_count = 3;
    int* dims = new int[layer_count] { 2, 3, 1 };
    int node_count = 0;
    for (int i = 0; i < layer_count; ++i)
    {
        node_count += dims[i];
    }
    node_count += layer_count;

    //Creation du model
    double* model = create_MLP_model(dims, layer_count);

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

    //-------------------------------------RBF-----------------------------------------
#if 0
    int* dims = new int[2] { 150, 2 };
    int inputSize = 1;
    float gamma = 0.1; //VALEUR MISE AU PIF

    double* model = create_RBF_model(dims, test.datasize);

    std::cout << "BEFORE TRAINING !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i++)
    {
        double* result = predict_RBF_model(model, dims, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, inputSize, test.datasize, isClassification, gamma);
        
        if(isClassification)
            std::cout << " resultat : " << result[0] << std::endl;
        else
        {
            std::cout << " resultat : " << std::endl;
            for (size_t i = 0; i < dims[1]; i++)
            {
                std::cout << result[i] << std::endl;
            }
        }
    }

    training_RBF_model(model, dims, test.samples, test.sample_count, inputSize, test.datasize, test.outputs, epoch, gamma);

    std::cout << "AFTER TRAINING !" << std::endl;
    for (int i = 0; i < test.sample_count; i++)
    {
        double* result = predict_RBF_model(model, dims, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, inputSize, test.datasize, isClassification, gamma);

        if (isClassification)
            std::cout << " resultat : " << result[0] << std::endl;
        else
        {
            std::cout << " resultat : " << std::endl;
            for (size_t i = 0; i < dims[1]; i++)
            {
                std::cout << result[i] << std::endl;
            }
        }
    }

#endif

    delete_model(model);
}