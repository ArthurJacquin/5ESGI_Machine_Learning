#include "Test.h"
#include <iostream>
#include <time.h>

CasTest::CasTest(TestType t)
{
	srand(time(NULL));
	switch (t)
	{
	case LinearSimple:
		sample_count = 3;
		samples = new double[sample_count * 2]{ 1.0, 1.0, 2.0, 3.0, 3.0, 3.0 };
		outputs = new double[sample_count] { 1, -1, -1 };
		break;

	case LinearMultiple:
		sample_count = 100;
		samples = new double[sample_count * 2];
		for (size_t i = 0; i < 200; i++)
		{
			if (i < 100)
				samples[i] = rand() / (double)RAND_MAX + 1.0;
			else
				samples[i] = rand() / (double)RAND_MAX + 2.0;
		}

		outputs = new double[sample_count];
		for (size_t i = 0; i < 100; i++)
		{
			if (i < 50)
				outputs[i] = 1.0;
			else
				outputs[i] = -1.0;
		}
		break;

	case XOR:
		sample_count = 4;
		samples = new double[sample_count * 2]{ 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 1.0 };
		outputs = new double[sample_count] { 1, 1, -1, -1 };
		break;

	case Cross:
		break;

	case MultiLinear:
		break;

	case MultiCross:
		break;

	case LinearSimple2D:
		sample_count = 2;
		samples = new double[sample_count * 2]{ 1.0, 2.0 };
		outputs = new double[sample_count] { 2, 3 };
		break;

	case NonLinearSimple2D:
		sample_count = 3;
		samples = new double[sample_count * 2]{ 1.0, 2.0, 3.0 };
		outputs = new double[sample_count] { 2, 3, 2.5 };
		break;

	case LinearSimple3D:
		sample_count = 3;
		samples = new double[sample_count * 2]{ 1.0, 1.0, 2.0, 2.0, 3.0, 1.0 };
		outputs = new double[sample_count] { 2, 3, 2.5 };
		break;

	case LinearTricky3D:
		sample_count = 3;
		samples = new double[sample_count * 2]{ 1.0, 1.0, 2.0, 2.0, 3.0, 3.0 };
		outputs = new double[sample_count] { 1, 2, 3 };
		break;

	case NonLinearSimple3D:
		sample_count = 4;
		samples = new double[sample_count * 2]{ 1.0, 0.0, 0.0, 1.0, 1.0, 1.0, 0.0, 0.0 };
		outputs = new double[sample_count] { 2, 1, -2, -1 };
		break;

	default:
		samples = new double[0];
		outputs = new double[0];
		break;
	}
}

void CasTest::DisplayInfos()
{
	std::cout.precision(5);

	std::cout << "Samples count : " << sample_count <<std::endl;
	std::cout << "   Samples     |      Output " << std::endl;
	for (size_t i = 0; i < sample_count; i++)
	{
		std::cout << samples[i * 2] << ", " << samples[i * 2 + 1] << " | " << outputs[i] << std::endl;
	}
}