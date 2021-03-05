
import ClientOAuth2 from "client-oauth2";

const auth = new ClientOAuth2({
  clientId: "f36b0fd1a2a3304a021b",
  clientSecret: "58c2c612a7d3b370aeab9c343dd46d576387fc9e",
  redirectUri: "http://localhost:32015/login",
  accessTokenUri: "https://github.com/login/oauth/access_token",
  authorizationUri: "https://github.com/login/oauth/authorize",
  scopes: [] // "read:user", "user:email"
});

console.log("uri",auth.code.getUri());



export default function Login() {
  return (
    <div>Login!</div>
  );
}
