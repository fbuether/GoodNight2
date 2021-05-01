import Oidc from "oidc-client";


var userManagerConfig = {
  "authority": "https://login.microsoftonline.com/common/v2.0",
  "client_id": "d8ee60f3-f059-4169-93b4-8faf1c32a9d8",
  "redirect_uri": "http://localhost:32015/finish-sign-in",
  "response_type": "id_token"
};


export interface User {
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

    return {
      name: user.id_token
    };
  }


  public async startSignIn() {
    await this.oidc.signinRedirect();
  }

  public async finishSignIn() {
    await this.oidc.signinRedirectCallback();
  }

  public async startSignOut() {
    await this.oidc.signoutRedirect();
  }

  public async finishSignOut() {
    await this.oidc.signoutRedirectCallback();
  }
}
