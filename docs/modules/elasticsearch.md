# Elasticsearch

[Elasticsearch](https://www.elastic.co/elasticsearch/) is a distributed, RESTful search and analytics engine capable of addressing a growing number of use cases. As the heart of the Elastic Stack, it centrally stores data for lightning fast search, fine‑tuned relevancy, and powerful analytics that scale with ease.

The following example uses the following NuGet dependencies:

<!--codeinclude-->
[Package References](../../tests/Testcontainers.Elasticsearch.Tests/Testcontainers.Elasticsearch.Tests.csproj) inside_block:PackageReferences
<!--/codeinclude-->

Copy and paste the following code into a new `.cs` test file within an existing test project.

<!--codeinclude-->
[Usage Example](../../tests/Testcontainers.Elasticsearch.Tests/ElasticsearchContainerTest.cs) inside_block:CreateElasticsearchContainer
<!--/codeinclude-->

To execute the tests, use the command `dotnet test` from a terminal.

## A Note To Developers

The Testcontainers module creates a container that listens to requests over **HTTPS**. To communicate with the Elasticsearch instance, developers must create a `ElasticsearchClientSettings` instance and set the `ServerCertificateValidationCallback` delegate to `CertificateValidations.AllowAll`. Failing to do so will result in a communication failure as the .NET will reject the certificate coming from the container.
