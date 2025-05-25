#include <cstddef>

class DynamicIntArray {
    int* data;
    size_t capacity;
    size_t length;
  
  public:
    DynamicIntArray(size_t initialCapacity = 1) {
      capacity = initialCapacity;
      length = 0;
      data = new int[capacity];
    }
  
    ~DynamicIntArray() {
      delete[] data;
    }
  
    void add(int value) {
      if (length >= capacity) {
        resize();
      }
      data[length++] = value;
    }
  
    int get(size_t index) const {
      if (index < length) return data[index];
      return -1;
    }
  
    size_t size() const {
      return length;
    }
  
  private:
    void resize() {
      capacity *= 2;
      int* newData = new int[capacity];
      for (size_t i = 0; i < length; i++) {
        newData[i] = data[i];
      }
      delete[] data;
      data = newData;
    }
  };
  