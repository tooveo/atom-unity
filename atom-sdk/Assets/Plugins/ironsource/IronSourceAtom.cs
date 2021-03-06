﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/*! \mainpage Atom SDK for Unity 3D
 *
 * \section intro_sec Introduction
 *
 * IronSource Atom is the official SDK for the Unity 3D.
 *
 */
namespace ironsource {
    internal class IronSourceCoroutineHandler : MonoBehaviour {
    }

    public class IronSourceAtom {
        protected static string API_VERSION_ = "V1.1.0";

        protected string endpoint_ = "https://track.atom-data.io/";
        protected string authKey_ = "";

        protected Dictionary<string, string> headers_ = new Dictionary<string, string>();
        protected GameObject parentGameObject_ = null;
        protected MonoBehaviour coroutineHandler_ = null;

        protected bool isDebug_ = false;

        /// <summary>
        /// API constructor
        /// </summary>
        /// <param name="gameObject">
        /// <see cref="GameObject"/> for coroutine method call.
        /// </param>
        public IronSourceAtom(GameObject gameObject = null) {
            parentGameObject_ = gameObject;

            initCoroutineHandler();
            initHeaders();
        }

        /// <summary>
        /// Enabling print debug information
        /// </summary>
        /// <param name="isDebug">
        /// If set to <c>true</c> is debug.
        /// </param>
        public void EnableDebug(bool isDebug) {
            isDebug_ = isDebug;
        }

        /// <summary>
        /// Check is Debug mode enabled
        /// </summary>
        /// <returns>
        /// Is Debug Mode
        /// </returns>
        public bool IsDebug() {
            return isDebug_;
        }

        /// <summary>
        /// Inits the coroutine handler.
        /// </summary>
        protected virtual void initCoroutineHandler() {
            coroutineHandler_ = parentGameObject_.GetComponent<MonoBehaviour>();
            if (coroutineHandler_ == null) {
                coroutineHandler_ = parentGameObject_.AddComponent<IronSourceCoroutineHandler>();
            }
        }

        /// <summary>
        /// Get coroutine handler.
        /// </summary>
        /// <returns>
        /// Coroutine handler as MonoBehaviour.
        /// </returns>
        public MonoBehaviour getCoruotinehandler() {
            return coroutineHandler_;
        }

        /// <summary>
        /// Inits the headers.
        /// </summary>
        protected virtual void initHeaders() {            
            headers_.Add("x-ironsource-atom-sdk-type", "unity");
            headers_.Add("x-ironsource-atom-sdk-version", IronSourceAtom.API_VERSION_);
        }

        /// <summary>
        /// Prints the log.
        /// </summary>
        /// <param name="logData">Log data.</param>
        protected void printLog(string logData) {
            if (isDebug_) {
                Debug.Log(logData);
            }
        }

        /// <summary>
        /// API destructor - clear craeted IronSourceCoroutineHandler
        /// </summary>       
        ~IronSourceAtom() {
            if (coroutineHandler_ != null) {
                UnityEngine.Object.Destroy(coroutineHandler_);
            }
        }

        /// <summary>
        /// Set Auth Key for stream
        /// </summary>  
        /// <param name="authKey">
        /// <see cref="string"/> for secret key of stream.
        /// </param>
        public virtual void SetAuth(string authKey) {
            authKey_ = authKey;
        }

        /// <summary>
        /// Set endpoint for send data
        /// </summary>
        /// <param name="endpoint">
        /// <see cref="string"/> for address of server
        /// </param>
        public void SetEndpoint(string endpoint) {
            endpoint_ = endpoint;
        }


        /// <summary>
        /// Send single data to Atom server.
        /// </summary>
        /// <param name="stream">
        /// Stream name for saving data in db table
        /// </param>
        /// <param name="data">
        /// user data to send
        /// </param>
        /// <param name="method">
        /// <see cref="HttpMethod"/> for POST or GET method for do request
        /// </param>
        /// <param name="callback">
        /// <see cref="string"/> for response data
        /// </param>
        public void PutEvent(string stream, string data, HttpMethod method = HttpMethod.POST, 
            Action<Response> callback = null) {
            string jsonEvent = GetRequestData(stream, data);
            SendEventCoroutine(endpoint_, method, headers_, jsonEvent, callback);
        }

        /// <summary>
        /// Send single data to Atom server.
        /// </summary>
        /// <param name="stream">
        /// <see cref="string"/> for name of stream
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> for request data
        /// </param>
        /// <param name="method">
        /// <see cref="HttpMethod"/> for type of request
        /// </param>
        /// <param name="callback">
        /// <see cref="string"/> for reponse data
        /// </param>
        /// <param name="parrentGameObject">
        /// <see cref="GameObject"/> for callback call.
        /// </param>
        public void PutEvent(string stream, string data, HttpMethod method = HttpMethod.POST, 
            string callback = null, GameObject parrentGameObject = null) {
            string jsonEvent = GetRequestData(stream, data);
            SendEventCoroutine(endpoint_, method, headers_, jsonEvent, callback, parrentGameObject);
        }

        /// <summary>
        /// Send multiple events data to Atom server.
        /// </summary>
        /// <param name="stream">
        /// <see cref="string"/> for name of stream
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> for request data
        /// </param>
        /// <param name="method">
        /// <see cref="HttpMethod"/> for type of request
        /// </param>
        /// <param name="callback">
        /// <see cref="Action<Response>"/> for reponse data
        /// </param>
        public void PutEvents(string stream, List<string> data, Action<Response> callback = null) {            
            string json = IronSourceAtomUtils.ListToJson(data);
            PutEvents(stream, json, callback);
        }

        public void PutEvents(string stream, string data, Action<Response> callback = null) {
            HttpMethod method = HttpMethod.POST;
            printLog("Key: " + authKey_);

            string jsonEvent = GetRequestData(stream, data);

            SendEventCoroutine(endpoint_ + "bulk", method, headers_, jsonEvent, callback);
        }

        /// <summary>
        /// Send multiple events data to Atom server.
        /// </summary>
        /// <param name="stream">
        /// <see cref="string"/> for name of stream
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> for request data
        /// </param>
        /// <param name="method">
        /// <see cref="HttpMethod"/> for type of request
        /// </param>
        /// <param name="callback">
        /// <see cref="string"/> for reponse data
        /// </param>
        /// <param name="parrentGameObject">
        /// <see cref="GameObject"/> for callback calling
        /// </param>
        public void PutEvents(string stream, List<string> data, string callback = null, 
            GameObject parentGameObject = null) {

            string json = IronSourceAtomUtils.ListToJson(data);
            PutEvents(stream, json, callback, parentGameObject);
        }

        public void PutEvents(string stream, string data, string callback = null, 
            GameObject parrentGameObject = null) {
            HttpMethod method = HttpMethod.POST;
            printLog("Key: " + authKey_);

            string jsonEvent = GetRequestData(stream, data);

            SendEventCoroutine(endpoint_ + "bulk", method, headers_, jsonEvent, callback, parrentGameObject);
        }

        /// <summary>
        /// Create request json data
        /// </summary>
        /// <returns>The request data.</returns>
        /// <param name="stream">
        /// <see cref="string"/> for request stream
        /// </param>
        /// <param name="data">
        /// <see cref="string"/> for request data
        /// </param>
        protected string GetRequestData(string stream, string data) {
            string hash = IronSourceAtomUtils.EncodeHmac(data, Encoding.ASCII.GetBytes(authKey_));

            var eventObject = new Dictionary<string, string>();
            eventObject ["table"] = stream;
            eventObject["data"] = IronSourceAtomUtils.EscapeStringValue(data);
            eventObject["auth"] = hash;
            string jsonEvent = IronSourceAtomUtils.DictionaryToJson(eventObject);

            printLog("Request body: " + jsonEvent);

            return jsonEvent;
        }

        /// <summary>
        /// Check health of server
        /// </summary>
        /// <param name="callback">
        /// <see cref="Action<Response>"/> for receive response from server
        /// </param>      
        public void Health(Action<Response> callback = null) {
            SendEventCoroutine(endpoint_ + "health", HttpMethod.GET, headers_, "", callback);
        }

        /// <summary>
        /// Send data to server
        /// </summary>
        /// <param name="url">
        /// <see cref="string"/> for server address
        /// </param>
        /// <param name="method">
        /// <see cref="HttpMethod"/> for POST or GET method 
        /// </param> 
        /// <param name="headers">
        /// <see cref="Dictionary<string, string>"/>
        /// </param> 
        /// <param name="data">
        /// <see cref="string"/> for request data
        /// </param> 
        /// <param name="callback">
        /// <see cref="Action<Response>"/> for receive response from server
        /// </param> 
        protected virtual void SendEventCoroutine(string url, HttpMethod method, Dictionary<string, string> headers,
            string data, Action<Response> callback) {

            Request request = new Request(url, data, headers, callback, isDebug_);
            if (method == HttpMethod.GET) {
                coroutineHandler_.StartCoroutine(request.Get());
            } else {
                coroutineHandler_.StartCoroutine(request.Post());
            }
        }

        /// <summary>
        /// Check health of server
        /// </summary>
        /// <param name="url">
        /// <see cref="string"/> for server address
        /// </param>
        /// <param name="method">
        /// <see cref="HttpMethod"/> for POST or GET method 
        /// </param> 
        /// <param name="headers">
        /// <see cref="Dictionary<string, string>"/>
        /// </param> 
        /// <param name="data">
        /// <see cref="string"/> for request data
        /// </param> 
        /// <param name="callback">
        /// <see cref="string"/> for receive response from server
        /// </param> 
        /// <param name="parrentGameObject">
        /// <see cref="GameObject"/> for calling callback
        /// </param>
        protected virtual void SendEventCoroutine(string url, HttpMethod method, Dictionary<string, string> headers,
            string data, string callback, GameObject parrentGameObject) {
            if (parrentGameObject == null) {
                parrentGameObject = parentGameObject_;
            }

            Request request = new Request(url, data, headers, callback, parrentGameObject, isDebug_);
            if (method == HttpMethod.GET) {
                coroutineHandler_.StartCoroutine(request.Get());
            } else {
                coroutineHandler_.StartCoroutine(request.Post());
            }   
        }
    }
}
