# UnityTemplates

## Setup GoogleSignIn

1. Create new [Project](https://console.cloud.google.com/projectcreate) (change Language at [here](https://myaccount.google.com/language))

2. Setup [OAuth consent screen](https://console.cloud.google.com/apis/credentials/consent) with Scopes :
 - ./auth/userinfo.email
 - ./auth/userinfo.profile
 - openid

3. Create [Credentials](https://console.cloud.google.com/apis/credentials) → OAuth client ID  → Web application  → Authorized redirect URIs : https://backendgame.com/GoogleSignIn.html
4. Web login → get Code :
   https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=profile%20email&redirect_uri=" + _redirect_uri + "&client_id=" + _webClientId
6. Transfer Code to id_token → POST : https://oauth2.googleapis.com/token
   - code
   - client_id
   - client_secret
   - redirect_uri : https://backendgame.com/GoogleSignIn.html
   - grant_type : authorization_code
   - requestUri : http://www.aioboardgame.com
7. id_token to infomation → GET https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=
