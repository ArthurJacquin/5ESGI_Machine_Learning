#pragma once
#include <cstdlib>

enum TestType {
	LinearSimple,
	LinearSimpleMulticlass,
	LinearMultiple,
	XOR,
	XORMulticlass,
	Cross,
	MultiLinear,
	MultiCross,
	LinearSimple2D,
	NonLinearSimple2D,
	LinearSimple3D,
	LinearTricky3D,
	NonLinearSimple3D
};

struct CasTest {

	int sample_count;
	int datasize;

	double* samples;
	double* outputs;

	CasTest(TestType t);

	void DisplayInfos();
};