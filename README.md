# Conjuror HTTP Service Virtualization

Conjuror is a HTTP [Service Virtualization] (https://www.infoq.com/articles/stubbing-mocking-service-virtualization-differences) server developed by @dalsoft it runs on ASP.NET Core 2.x.

# Why would I use Conjuror?

You use Conjuror as a HTTP server to monitor HTTP requests and control HTTP responses. Typically you would want to do this in your accepatnce tests for example: you might be testing a production build of your iOS app that has a REST API, you want to run UI tests against certain states that the API returns but it's difficult to trigger those states normally, or maybe you don't want to poluate the code with lots of mocks.

# Getting Started

Conjuror is a powerful middleware that can run as a standalone server or within your ASP.NET stack, that you may want only virtualize a particalur resource. Below are tripical examples of running both standalone and as part your ASP.NET stack.

