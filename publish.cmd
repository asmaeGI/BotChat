nuget restore
msbuild BasicBot.sln -p:DeployOnBuild=true -p:PublishProfile=sqlicustomerservicebot-Web-Deploy.pubxml -p:Password=BDMunkrJMbD64Hce9vozrFmg7BxPukMduXuWAlrB4oajD7u66RCA3fut9Bmt

