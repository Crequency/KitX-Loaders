//
// Created by Dynesshely on 2023.01.24.
//

#include "ip_address.h"

namespace KitX::Loaders::Cpp::Self::Include{

    void ip_address::v4Set(char a, char b, char c, char d) {
        v4_A = a, v4_B = b, v4_C = c, v4_D = d;
    }

    int ip_address::v4Set(int index, char value) {
        auto len = 4;
        auto ip = new char*[len];
        ip[0] = &v4_A, ip[1] = &v4_B, ip[2] = &v4_C, ip[3] = &v4_D;
        if(index >= 0 && index < len)
            *ip[index] = value;
        else return -1;
        return 0;
    }
}
