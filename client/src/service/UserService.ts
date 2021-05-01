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

    // Oidc.Log.logger = console;
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

    return {
      email: user.profile.email,
      name: user.profile.sub
    };
  }

  public async startSignIn() {
    await this.oidc.signinRedirect();
  }

  public async finishSignIn() {
    try {
      await this.oidc.signinRedirectCallback(window.location.href);
    }
    catch (err) {
      console.log("Error while finishing sign in:", err);
      throw err;
    }
  }


  public async removeUser() {
    await this.oidc.removeUser();
  }


  // public async startSignOut() {
  //   await this.oidc.signoutRedirect();
  // }

  // public async finishSignOut() {
  //   await this.oidc.signoutRedirectCallback();
  // }
}
