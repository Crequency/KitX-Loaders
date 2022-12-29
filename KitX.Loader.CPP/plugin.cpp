#include "loader.h"


int main(int argc, std::string argv[] )
{
    try
    {
        std::string address;
        int port;
        KitX::Plugin myplugin("myname", "myauthname", "1.0.0", "mydescription", "myauthor", "mywebsite", "mypluginpath");
        myplugin.beginConnect("192.168.1.8", 64094);
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }
    int a, b;
    std::cin >> a >> b;
    std::cout << a + b << " \n";
    return 0;
}

