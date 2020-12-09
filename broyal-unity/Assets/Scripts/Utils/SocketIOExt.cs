using System;
using System.Linq;
using FullSerializer;
using Scripts.Common.Data;
using Scripts.Scenes.Lobby;
using SocketIO;
using SocketIO.Data.Responses;

namespace SocketIOExt
{
    public static class EventSerialize
    {
        static fsSerializer serializer = new fsSerializer();

        public static string ToJson<T>(this T @event, bool prettyJson = false) where T : class
        {
            var result = serializer.TrySerialize(@event, out fsData fsData);
        
            if (result.Succeeded)
            {
                return prettyJson ? fsJsonPrinter.PrettyJson(fsData) : fsJsonPrinter.CompressedJson(fsData);
            }
            else
            {
                throw result.AsException;
            }
        }

        public static T Deserialize<T>(this string data) where T : class, new()
        {
            var result = fsJsonParser.Parse(data, out fsData fsData);

            if (result.Succeeded)
            {
                var response = new T();
                result = serializer.TryDeserialize(fsData, ref response);

                return result.Succeeded ? response : throw result.AsException;
            }
            else
            {
                throw result.AsException;
            }
        }

        public static BaseResponse ToBaseResponse(this string data)
        {
            var result = fsJsonParser.Parse(data, out fsData fsData);

            if (result.Succeeded)
            {
                var response = new BaseResponse();
                result = serializer.TryDeserialize(fsData, ref response);

                return result.Succeeded ? response : null;
            }
            else
            {
                return null;
            }
        }

        // public static BaseEvent ToEventResponse(this string data)
        // {
        //     var result = fsJsonParser.Parse(data, out fsData fsData);
        //
        //     if (result.Succeeded)
        //     {
        //         var response = new BaseEvent();
        //         result = serializer.TryDeserialize(fsData, ref response);
        //
        //         return result.Succeeded ? response : null;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }
        
        public static void Emit<TR>(this SocketIOComponent socket, string ev, string data, Action<TR> callback)
            where TR : BaseResponse, new()
        {
            socket.Emit(ev, data, (obj) => callback(obj.Deserialize<TR>()) );
        }
        
        public static void Emit<TR>(this SocketIOComponent socket, string ev, JSONObject data, Action<TR> callback)
            where TR : BaseResponse, new()
        {
            socket.Emit(ev, data, (obj) => callback(obj.Deserialize<TR>()) );
        }
        
        public static TR Deserialize<TR>(this JSONObject objectResponse)
            where TR : class, new()
        {
            var json = objectResponse.ToString();
            if (json.StartsWith("["))
            {
                json = json.TrimStart(new char[] {'['});
                json = json.TrimEnd(new char[] {']'});
            }

            return json.Deserialize<TR>();
        }
        
        public static void LoginWithDeviceId(this SocketIOComponent socket, string deviceId, Action<SocketIO.Data.Responses.User> onSuccess, Action onError)
        {
            socket.Emit<VerifyResponse>(LobbyEvents.LOGIN_WITH_DEVICEID, deviceId, (response) =>
            {
                if (response.IsSuccess) onSuccess?.Invoke(response.data.user);
                else onError?.Invoke();
            });
        }
        
        public static void GetCharacters(this SocketIOComponent socket, string id, Action<Character[]> onSuccess, Action onError)
        {
            socket.Emit<CharactersResponse>(LobbyEvents.GET_CHARACTERS, id, (response) =>
            {
                if (response.IsSuccess) onSuccess?.Invoke(response.data.characters);
                else onError?.Invoke();
            });
        }
        
        public static void SetCharacter(this SocketIOComponent socket, Character character, Action<BaseResponse> onSuccess, Action onError)
        {
            var json = character.ToJson();
            var jsObject = new JSONObject(json);

            socket.Emit<BaseResponse>(LobbyEvents.SET_CHARACTER, jsObject, (response) =>
            {
                if (response.IsSuccess) onSuccess?.Invoke(response);
                else onError?.Invoke();
            });
        }
        
        public static void CreateGame(this SocketIOComponent socket, string gameName, Action<CreateGameResponse.Data> onSuccess, Action onError)
        {
            socket.Emit<CreateGameResponse>(LobbyEvents.CREATE_GAME, gameName, (response) =>
            {
                if (response.IsSuccess) onSuccess?.Invoke(response.data);
                else onError?.Invoke();
            });
        }
        
        // public static void StartGame(this SocketIOComponent socket, string gameId, Action<StartGameResponse.Data> onSuccess, Action onError)
        // {
        //     socket.Emit<StartGameResponse>(LobbyEvents.START_GAME, gameId, (response) =>
        //     {
        //         if (response.IsSuccess) onSuccess?.Invoke(response.data);
        //         else onError?.Invoke();
        //     });
        // }
        
        public static void GetGames(this SocketIOComponent socket, Action<UpdateGamesListResponse.Data> onSuccess, Action onError)
        {
            socket.Emit<UpdateGamesListResponse>(LobbyEvents.GET_GAMES,"", (response) =>
            {
                if (response.IsSuccess) onSuccess?.Invoke(response.data);
                else onError?.Invoke();
            });
        }
        
        public static void JoinGame(this SocketIOComponent socket, string gameId, Action<JoinGameResponse.Data> onSuccess, Action onError)
        {
            socket.Emit<JoinGameResponse>(LobbyEvents.JOIN_GAME, gameId, (response) =>
            {
                if (response.IsSuccess) onSuccess?.Invoke(response.data);
                else onError?.Invoke();
            });
        }
    }
}