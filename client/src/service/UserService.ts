import Oidc from "oidc-client";


var userManagerConfig = {
  "authority": "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0",
  "client_id": "d8ee60f3-f059-4169-93b4-8faf1c32a9d8",
  "redirect_uri": "http://localhost:32015/finish-sign-in",
  "response_type": "code",
  "response_mode": "query"
};


export interface User {
  email: string | undefined;
  name: string;
}


export class UserService {
  private oidc: Oidc.UserManager;

  private static instance: UserService | null = null;

  private constructor() {
    if (UserService.instance != null) {
      throw new Error("Created second instance of UserService!");
    }

    Oidc.Log.logger = console;
    this.oidc = new Oidc.UserManager(userManagerConfig);
  }

  public static get(): UserService {
    if (UserService.instance == null) {
      UserService.instance = new UserService();
    }

    return UserService.instance;
  }


  public async getUser(): Promise<User | null> {
    var user = await this.oidc.getUser();
    if (user == null) {
      return null;
    }

    console.log("user", user);

    return {
      email: user.profile.email,
      name: user.profile.sub
    };
  }


  // public async quietSignIn() {
  //   // return await this.oidc.signinSilent();
  // }


  public async startSignIn() {
    console.log("userservice", this);

    await this.oidc.signinRedirect();
  }

  public async finishSignIn() {
    // console.log("window location hash", window.location, decodeURIComponent(window.location));

    // if (window.location.hash) {
    //   window.location.hash = decodeURIComponent(window.location.hash);
    //   // authorizedCallback returns wrong result when hash is URI encoded
    //   this.oidcSecurityService.authorizedCallback();
    // } else {
    //   this.oidcSecurityService.authorize();
    // }

    try {
      console.log("finish login with: ", window.location.href);
      let user = await this.oidc.signinRedirectCallback(window.location.href);
      console.log("got user:", user);
    }
    catch (err) {
      console.log("got error", err);
    }
  }

  public async startSignOut() {
    // await this.oidc.signoutRedirect();
    await this.oidc.removeUser();
  }

  // public async finishSignOut() {
  //   await this.oidc.signoutRedirectCallback();
  // }
}
