using System;
using System.Net;
using NUnit.Framework;
using Rackspace.CloudFiles.Domain.Request;
using Rackspace.CloudFiles.Exceptions;


namespace Rackspace.CloudFiles.Integration.Tests.domain.GetStorageItemInformationSpecs
{
    [TestFixture]
    public class When_getting_information_on_a_storage_item : TestBase
    {
        [Test]
        public void Should_get_200_OK_or_204_No_Content_when_item_exists()
        {
            
            using (var testHelper = new TestHelper(authToken, storageUrl))
            {
                try
                {
                    testHelper.PutItemInContainer(Constants.HeadStorageItemName);
                    testHelper.AddMetadataToItem(Constants.HeadStorageItemName);

                    var getStorageItemInformation = new GetStorageItemInformation(storageUrl, Constants.CONTAINER_NAME, Constants.HeadStorageItemName);
                    var getStorageItemInformationResponse = new GenerateRequestByType().Submit(
                        getStorageItemInformation, authToken);
                    Assert.That(getStorageItemInformationResponse.Status == HttpStatusCode.OK 
                        || getStorageItemInformationResponse.Status == HttpStatusCode.NoContent, Is.True);

                    var metadata = getStorageItemInformationResponse.Metadata;
                    Assert.That(metadata["Test"], Is.EqualTo("test"));
                    Assert.That(metadata["Test2"], Is.EqualTo("test2"));
                }
                finally
                {
                    testHelper.DeleteItemFromContainer(Constants.HeadStorageItemName);
                }
            }
        }

        [Test]
        public void Should_get_404_when_item_does_not_existt()
        {
            

            using (new TestHelper(authToken, storageUrl))
            {
                var getStorageItemInformation = new GetStorageItemInformation(storageUrl, Constants.CONTAINER_NAME, Constants.StorageItemName);
                try
                {
                    new GenerateRequestByType().Submit(getStorageItemInformation, authToken);
                }
                catch (Exception ex)
                {
                    Assert.That(ex, Is.TypeOf(typeof (WebException)));
                }
            }
        }

        [Test]
        [ExpectedException(typeof (ContainerNameException))]
        public void Should_throw_an_exception_when_the_length_of_the_container_name_exceeds_the_maximum_allowed_length()
        {
            new GetStorageItemInformation("a", new string('a', Constants.MaximumContainerNameLength + 1), "a");
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Should_throw_an_exception_when_the_storage_url_is_null()
        {
            new GetStorageItemInformation(null,  "a", "a");
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Should_throw_an_exception_when_the_container_name_is_null()
        {
            new GetStorageItemInformation("a",  null, "a");
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Should_throw_an_exception_when_the_storage_object_name_is_null()
        {
            new GetStorageItemInformation("a",  "a", null);
        }

      
    }
}