//
// Created by Dynesshely on 2023.01.24.
//

#ifndef KITX_LOADER_CPP_SELF_IP_ADDRESS_HPP
#define KITX_LOADER_CPP_SELF_IP_ADDRESS_HPP

namespace KitX::Loaders::Cpp::Self {
    class ip_address {
    private:
        char v4_A, v4_B, v4_C, v4_D;
    public:
        void v4Set(char a, char b, char c, char d);

        int v4Set(int index, char value);

        char* v4toCharArray();
    };
}


#endif //KITX_LOADER_CPP_SELF_IP_ADDRESS_HPP
