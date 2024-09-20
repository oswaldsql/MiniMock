namespace MiniMock.Tests.Asserts;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

public class AssertMethodCallTest
{
    /// <summary>
    ///  Represents a library that can download versions.
    /// </summary>
    public interface IAssertMethodCall
    {
/*        /// <summary> Gets or sets the current version of the library. </summary>
        Version CurrentVersion { get; set; }
*/
        /// <summary> Gets a value indicating whether a download exists for the specified version. </summary>
        /// <param name="version">The version as a <c>string</c></param>
        /// <returns><c>true</c> if exists, otherwise <c>false</c></returns>
        DateTime ReleaseDate(string version, bool isPreview);
/*
        /// <summary> Gets a value indicating whether a download exists for the specified version. </summary>
        /// <param name="version">The version</param>
        /// <returns><c>true</c> if exists, otherwise <c>false</c></returns>
        bool DownloadExists(Version version);

        /// <summary>
        /// Gets the download link for the specified version.
        /// </summary>
        /// <param name="version">The version</param>
        /// <returns>The uri to the specified version</returns>
        Task<Uri> DownloadLinkAsync(string version);

        /// <summary>
        ///  Gets the versoion for the specified key.
        /// </summary>
        /// <param name="key">The version key.</param>
        Version this[string key] { get; set; }

        /// <summary>
        ///  Occurs when a new version is added.
        /// </summary>
        event EventHandler<Version> NewVersionAdded;
        */
    }

    [Fact]
    [Mock<IAssertMethodCall>]
    public void METHOD()
    {
        // Arrange
        var sut = Mock.IAssertMethodCall(config => config
                .ReleaseDate(assert: (version, preview) => version == "1.0" && preview)
                .ReleaseDate(call: (version, preview) => DateTime.UtcNow)
            //.ReleaseDate(assert: (Version version) => true)
        );

        // ACT
        Assert.Throws<System.ArgumentException>(() =>sut.ReleaseDate("2.0", false));
        // Assert

    }

}

