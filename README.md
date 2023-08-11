# UnityTemplates

## Setup GoogleSignIn

Create new [Project](https://console.cloud.google.com/projectcreate) (change Language at [here](https://myaccount.google.com/language))
Setup [OAuth consent screen](https://console.cloud.google.com/apis/credentials/consent) with Scopes :
 - ./auth/userinfo.email
 - ./auth/userinfo.profile
 - openid

Create [Credentials](https://console.cloud.google.com/apis/credentials) → OAuth client ID  → Web application  → Authorized redirect URIs : https://backendgame.com/GoogleSignIn.html
