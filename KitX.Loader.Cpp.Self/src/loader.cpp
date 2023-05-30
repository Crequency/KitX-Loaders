#include <loader.hpp>

namespace KitX::Loaders::Cpp::Self{

    void Init(Contract::Cpp::IIdentifyInterface *iidi){
        identifyInterface = iidi;
    }

    int BeginConnect(ip_address address, short port){



        return 0;
    }
}
