#include <vector>
#include <string>
#include <algorithm>

extern "C" void SortStrings(char* strings[], int length)
{
    std::vector<std::string> vec(strings, strings + length);
    std::sort(vec.begin(), vec.end());
    for (int i = 0; i < length; ++i)
    {
        strcpy(strings[i], vec[i].c_str());
    }
}