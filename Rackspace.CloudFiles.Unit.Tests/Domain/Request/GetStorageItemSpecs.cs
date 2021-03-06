using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Rackspace.CloudFiles.Domain.Request;
using Rackspace.CloudFiles.Domain.Request.Interfaces;
using Rackspace.CloudFiles.Exceptions;
using Rackspace.CloudFiles.Utils;


namespace Rackspace.CloudFiles.Unit.Tests.Domain.Request
{
    [TestFixture]
    public class When_getting_a_storage_object
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_an_exception_when_the_storage_url_is_null()
        {
            new GetStorageItem(null, "a", "a");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_an_exception_when_the_container_bame_is_null()
        {
            new GetStorageItem("a", null, "a");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_an_exception_when_the_storage_object_name_is_null()
        {
            new GetStorageItem("a", "a", null);
        }

     
    }

    [TestFixture]
    public class When_getting_a_storage_item_and_providing_a_if_match_request_header
    {
        private Dictionary<RequestHeaderFields, string> requestHeaderFields;
        private const string DUMMY_ETAG = "5c66108b7543c6f16145e25df9849f7f";

        [SetUp]
        public void SetUp()
        {
            requestHeaderFields = new Dictionary<RequestHeaderFields, string>();
            requestHeaderFields.Add(RequestHeaderFields.IfMatch, DUMMY_ETAG);
        }

        [Test]
        public void Should_add_if_match_request_field_header_to_request_successfully()
        {
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "NotEmptyString", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            Asserts.AssertHeaders(getStorageItem, EnumHelper.GetDescription(RequestHeaderFields.IfMatch), DUMMY_ETAG);

        }
    }

    [TestFixture]
    public class When_getting_a_storage_item_and_providing_a_if_none_match_request_header
    {
        private Dictionary<RequestHeaderFields, string> requestHeaderFields;
        private const string DUMMY_ETAG = "5c66108b7543c6f16145e25df9849f7fTest";

        [SetUp]
        public void SetUp()
        {
            requestHeaderFields = new Dictionary<RequestHeaderFields, string>();
            requestHeaderFields.Add(RequestHeaderFields.IfNoneMatch, DUMMY_ETAG);
        }

        [Test]
        public void Should_add_if_none_match_request_field_header_to_request_successfully()
        {
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "NotEmptyString", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            Asserts.AssertHeaders(getStorageItem, EnumHelper.GetDescription(RequestHeaderFields.IfNoneMatch), DUMMY_ETAG);
            
            
        }
    }

    [TestFixture]
    public class When_getting_a_storage_item_and_providing_a_if_modified_since_request_header
    {
        private Dictionary<RequestHeaderFields, string> requestHeaderFields;
        private DateTime modifiedDateTime = DateTime.Now.AddMinutes(-5);

        [SetUp]
        public void SetUp()
        {
            requestHeaderFields = new Dictionary<RequestHeaderFields, string>();
            requestHeaderFields.Add(RequestHeaderFields.IfModifiedSince, modifiedDateTime.ToString());
        }

        [Test]
        [ExpectedException(typeof(DateTimeHttpHeaderFormatException))]
        public void Should_throw_is_modified_since_header_exception_if_date_time_provided_is_not_even_a_date()
        {
            requestHeaderFields[RequestHeaderFields.IfModifiedSince] = "test_jibberish";

            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            Asserts.AssertHeaders(getStorageItem, EnumHelper.GetDescription(RequestHeaderFields.IfModifiedSince), modifiedDateTime);
           
        }

        [Test]
        [Ignore("should write hand rolled mock")]
        public void Should_add_if_modified_since_request_field_to_request_ifmodifiedsince_property_successfully()
        {
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            var request = Asserts.GetMock(getStorageItem);

            request.VerifySet(x=>x.IfModifiedSince= modifiedDateTime);
           // Assert.That(request.ModifiedSince.ToShortDateString(), Is.EqualTo(modifiedDateTime.ToShortDateString()));
            //Assert.That(request.ModifiedSince.ToShortTimeString(), Is.EqualTo(modifiedDateTime.ToShortTimeString()));
        }
    }

    [TestFixture]
    public class When_getting_a_storage_item_and_providing_a_if_unmodified_since_request_header
    {
        private Dictionary<RequestHeaderFields, string> requestHeaderFields;
        private DateTime modifiedDateTime = DateTime.Now.AddMinutes(-5);

        [SetUp]
        public void SetUp()
        {
            requestHeaderFields = new Dictionary<RequestHeaderFields, string>();
            requestHeaderFields.Add(RequestHeaderFields.IfUnmodifiedSince, modifiedDateTime.ToString());
        }

        [Test]
        [ExpectedException(typeof(DateTimeHttpHeaderFormatException))]
        public void Should_throw_date_time_http_header_format_exception_if_date_time_provided_is_not_even_a_date()
        {
            requestHeaderFields[RequestHeaderFields.IfModifiedSince] = "test_jibberish";

            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            Asserts.AssertHeaders(getStorageItem, EnumHelper.GetDescription(RequestHeaderFields.IfUnmodifiedSince), modifiedDateTime);

        }

        [Test]
        public void Should_add_if_unmodified_since_request_field_to_request_ifmodifiedsince_property_successfully()
        {
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            Asserts.AssertHeaders(getStorageItem, EnumHelper.GetDescription(RequestHeaderFields.IfUnmodifiedSince), String.Format("{0:r}", modifiedDateTime));
          //  Assert.That(getStorageItem.Headers[EnumHelper.GetDescription(RequestHeaderFields.IfUnmodifiedSince)], Is.EqualTo(String.Format("{0:r}", modifiedDateTime)));
        }
    }

    [TestFixture]
    public class When_getting_a_storage_item_and_providing_a_range_header
    {
        private Dictionary<RequestHeaderFields, string> requestHeaderFields;

        [SetUp]
        public void SetUp()
        {
            requestHeaderFields = new Dictionary<RequestHeaderFields, string>();
        }

        [Test]
        [ExpectedException(typeof(InvalidRangeHeaderException))]
        public void Should_throw_invalid_range_exception_if_the_from_range_is_not_an_integer()
        {
            requestHeaderFields[RequestHeaderFields.Range] = "a-5";

            new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields).Apply(new Mock<ICloudFilesRequest>().Object);
        }

        [Test]
        public void Should_have_a_range_from_property_if_the_range_from_property_is_set_correctly()
        {
            requestHeaderFields[RequestHeaderFields.Range] = "10-";
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            var request = Asserts.GetMock(getStorageItem);
            request.VerifySet(x=>x.RangeFrom=10);
           // Assert.That(request.RangeFrom, Is.EqualTo(10));
        }

        [Test]
        public void Should_have_a_negative_range_to_property_if_the_range_to_property_is_set_correctly_and_no_range_from_is_specified()
        {
            requestHeaderFields[RequestHeaderFields.Range] = "-10";
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            var request = Asserts.GetMock(getStorageItem);
            request.VerifySet(x => x.RangeTo = -10);
            // Assert.That(getStorageItem.RangeTo, Is.EqualTo(-10));
        }

        [Test]
        public void Should_have_range_from_and_range_to_if_both_are_set_and_are_valid_integers()
        {
            requestHeaderFields[RequestHeaderFields.Range] = "1-10";
            GetStorageItem getStorageItem = new GetStorageItem("http://storageurl", "containername", Constants.STORAGE_ITEM_NAME, requestHeaderFields);
            var request = Asserts.GetMock(getStorageItem);
            request.VerifySet(x => x.RangeFrom = 1);
            request.VerifySet(x => x.RangeTo = 10);
            //Assert.That(getStorageItem.RangeFrom, Is.EqualTo(1));   
           // Assert.That(getStorageItem.RangeTo, Is.EqualTo(10));
        }
    }
}