﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Helpers
{
    public class DevHttpsConnectionHelper
    {

        public DevHttpsConnectionHelper(int sslPort)
        {
            SslPort = sslPort;
            DevServerRootUrl = FormattableString.Invariant($"https://{DevServerName}:{SslPort}");
            LazyHttpClient = new Lazy<HttpClient>(() =>
            {
#if ANDROID
                return new HttpClient(GetPlatformMessageHandler());
#else
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                return new HttpClient(clientHandler);

#endif
            });
        }

        public int SslPort { get; }

        public string DevServerName =>
#if WINDOWS
        //"localhost";
        "192.168.1.6";
#elif ANDROID
            //"10.0.2.2";
            "192.168.1.6";
#else
        throw new PlatformNotSupportedException("Only Windows and Android currently supported.");
#endif

        public string DevServerRootUrl { get; }

        private Lazy<HttpClient> LazyHttpClient;
        public HttpClient HttpClient => LazyHttpClient.Value;

        public HttpMessageHandler? GetPlatformMessageHandler()
        {
#if WINDOWS
        return null;
#elif ANDROID
        var handler = new CustomAndroidMessageHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert != null && cert.Issuer.Equals("CN=localhost"))
                return true;
            return errors == SslPolicyErrors.None;
        };
        return handler;
#else
            throw new PlatformNotSupportedException("Only Windows and Android currently supported.");
#endif
        }

#if ANDROID 
        internal sealed class CustomAndroidMessageHandler : Xamarin.Android.Net.AndroidMessageHandler
        {
            protected override Javax.Net.Ssl.IHostnameVerifier GetSSLHostnameVerifier(Javax.Net.Ssl.HttpsURLConnection connection)
                => new CustomHostnameVerifier();

            private sealed class CustomHostnameVerifier : Java.Lang.Object, Javax.Net.Ssl.IHostnameVerifier
            {
                public bool Verify(string? hostname, Javax.Net.Ssl.ISSLSession? session)
                {
                    return
                        Javax.Net.Ssl.HttpsURLConnection.DefaultHostnameVerifier.Verify(hostname, session)
                        || (hostname == "10.0.2.2" || hostname == "192.168.1.6") && session.PeerPrincipal?.Name == "CN=localhost";
                }
            }
        }
#endif
    }
}
