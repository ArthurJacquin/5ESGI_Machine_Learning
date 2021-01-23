#pragma once
#include <cstdlib>

enum TestType {
	LinearSimple,
	LinearSimpleMulticlass,
	LinearMultiple,
	LinearMultipleMulticlass,
	XOR,
	XORMulticlass,
	Cross,
	CrossMulticlass,
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