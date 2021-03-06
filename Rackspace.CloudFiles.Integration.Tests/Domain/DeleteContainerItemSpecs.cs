using System;
using System.Net;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Rackspace.CloudFiles.Domain.Request;
using Rackspace.CloudFiles.Exceptions;


namespace Rackspace.CloudFiles.Integration.Tests.domain.DeleteStorageObjectSpecs
{
    [TestFixture]
    public class When_deleting_a_storage_item : TestBase
    {
        [Test]
        public void should_return_204_no_content_when_the_item_exists()
        {
            
            using (TestHelper testHelper = new TestHelper(authToken, storageUrl))
            {
                testHelper.PutItemInContainer();

                var deleteStorageItem = new DeleteStorageItem(storageUrl, Constants.CONTAINER_NAME, Constants.StorageItemName);
                var response = new GenerateRequestByType().Submit(deleteStorageItem,authToken );

                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.NoContent));
                Console.WriteLine(response.Headers["Content-Type"]);
                Assert.That(Regex.Match(response.Headers["Content-Type"], "text/(plain|html)").Success, Is.True);
            }
        }

        [Test]
        public void Shoulds_return_404_when_the_item_does_not_exist()
        {
            
            using (new TestHelper(authToken, storageUrl))
            {
                var deleteStorageItem = new DeleteStorageItem(storageUrl, Constants.CONTAINER_NAME, Guid.NewGuid().ToString());
                try
                {
                  
                   new GenerateRequestByType().Submit(deleteStorageItem, authToken);
                   
                }
                catch (Exception ex)
                {
                    Assert.That(ex, Is.TypeOf(typeof (WebException)));
                }
            }
        }

        [Test]
        public void Should_throw_an_exception_when_the_container_name_length_exceeds_the_maximum_allowed_length()
        {
            string containerName = new string('a', Constants.MaximumContainerNameLength + 1);
            try
            {
                using (new TestHelper(authToken, storageUrl, containerName))
                {
                }
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.TypeOf(typeof (ContainerNameException)));
                Assert.That(ex, Is.TypeOf(typeof (ContainerNameException)));
            }
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Should_throw_an_exception_when_the_container_name_is_null()
        {
            new DeleteStorageItem("a",  null, "a");
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Should_throw_an_exception_when_the_storage_url_name_is_null()
        {
            new DeleteStorageItem(null, "a", "a");
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Should_throw_an_exception_when_the_storage_object_name_is_null()
        {
            new DeleteStorageItem("a",  "a", null);
        }

      
    }
}