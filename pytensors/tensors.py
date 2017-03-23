def copyTensor(original, dim):
	"""========================================
Make a deep copy of a tensor of arbitrary (number of) dimensions.

Usage example, to deep-copy a two-dimensional matrix:
>>> p = [ [1, 2], [3, 4] ]
>>> deep = copyTensor(p, 2)

It can also make a shallow copy, if you pass a number of dimensions less than the actual one:
>>> shallow = copyTensor(p, 1)
========================================"""
	copy = []
	if dim == 1:
		for element in original:
			copy.append(element)
	elif dim > 1:
		for list in original:
			copy.append(copyTensor(list, dim - 1))	
	return copy


def initTensor(value, *lengths):
	"""========================================
Initialize uniformly a tensor of arbitrary (number of) dimensions.

Usage example:
>>> print initTensor(0, 3, 4)
[[0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0]]
>>> print initTensor(0, 2, 3, 4)
[[[0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0]], [[0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0]]]
========================================"""
	list = []
	dim = len(lengths)
	if dim == 1:
		for i in range(lengths[0]):
			list.append(value)
	elif dim > 1:
		for i in range(lengths[0]):
			list.append(initTensor(value, *lengths[1:]))
	return list
