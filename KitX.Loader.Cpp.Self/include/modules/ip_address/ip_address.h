//
// Created by Dynesshely on 2023.01.24.
//

#ifndef KITX_LOADER_CPP_SELF_IP_ADDRESS_H
#define KITX_LOADER_CPP_SELF_IP_ADDRESS_H

namespace KitX::Loaders::Cpp::Self::Include {
    class ip_address {
    private:
        char v4_A, v4_B, v4_C, v4_D;
    public:
        void v4Set(char a, char b, char c, char d);
        int v4Set(int index, char value);
    };
}


#endif //KITX_LOADER_CPP_SELF_IP_ADDRESS_H
