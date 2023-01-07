#ifndef KITX_CONTRACT_CPP_KITX_CONTRACT_H
#define KITX_CONTRACT_CPP_KITX_CONTRACT_H

#include <string>
#include <hash_map>
#include <utility>
#include <chrono>

namespace KitX::Contract::Cpp
{
    typedef std::unordered_map<std::string, std::string> Dictionary;
    typedef std::string string;
    typedef void(*fp);
    typedef void *(*fps)(std::string cmd);
    typedef void *(*fpso)(std::string cmd, void *arg);
    class DateTime
    {
    private:
        time_t time_sec;
    public:
        DateTime(time_t now = time(0)):time_sec(now){}
    };
    class Function
    {
    };
    struct IController
    {
    };
    struct IMarketPluginContract
    {
        fp Start;
        fp Pause;
        fp End;
        Function **Functions;
        fps Execute;
        fpso ExecuteWithArg;
        fps SetRootPath;
        fps SetWorkPath;
    };
    struct IIdentifyInterface
    {
        IIdentifyInterface(string mName,
                           string mVersion,
                           Dictionary mDisplayName,
                           string mAuthorName,
                           string mPublisherName,
                           string mAuthorLink,
                           string mPublisherLink,
                           Dictionary mSimpleDescription,
                           Dictionary mComplexDescription,
                           Dictionary mTotalDescriptionInMarkdown,
                           string mIconInBase64,
                           DateTime mPublishDate,
                           DateTime mLastUpdateDate,
                           IController mController,
                           bool mIsMarketVersion,
                           IMarketPluginContract mMarketPluginContract,
                           string mRootStartupFileName)
            : Name(std::move(mName)),
              Version(std::move(mVersion)),
              AuthorName(std::move(mAuthorName)),
              DisplayName(std::move(mDisplayName)),
              PublisherName(std::move(mPublisherName)),
              AuthorLink(std::move(mAuthorLink)),
              PublisherLink(std::move(mPublisherLink)),
              SimpleDescription(std::move(mSimpleDescription)),
              ComplexDescription(std::move(mComplexDescription)),
              PublishDate(mPublishDate),
              TotalDescriptionInMarkdown(std::move(mTotalDescriptionInMarkdown)),
              IconInBase64(std::move(mIconInBase64)),
              IsMarketVersion(mIsMarketVersion),
              RootStartupFileName(std::move(mRootStartupFileName)),
              LastUpdateDate(mLastUpdateDate),
              Controller(mController),
              MarketPluginContract(mMarketPluginContract)
        {
        }
        string Name;
        string Version;
        Dictionary DisplayName;
        string AuthorName;
        string PublisherName;
        string AuthorLink;
        string PublisherLink;
        Dictionary SimpleDescription;
        Dictionary ComplexDescription;
        Dictionary TotalDescriptionInMarkdown;
        string IconInBase64;
        DateTime PublishDate;
        DateTime LastUpdateDate;
        IController Controller;
        bool IsMarketVersion;
        IMarketPluginContract MarketPluginContract;
        string RootStartupFileName;
        void operator=(IIdentifyInterface s)
        {
            this->Name = s.Name;
            this->Version = s.Version;
            this->DisplayName = s.DisplayName;
            this->AuthorName = s.AuthorName;
        }
    };

}

#endif // KITX_CONTRACT_CPP_KITX_CONTRACT_H
