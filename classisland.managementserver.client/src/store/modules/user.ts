import { defineStore } from 'pinia';
import { store } from '@/store';
import {
  ACCESS_TOKEN,
  CURRENT_USER,
  IS_SCREENLOCKED,
  REFRESH_TOKEN,
  TOKEN_EXPIRE_TIME
} from '@/store/mutation-types';
import { ResultEnum } from '@/enums/httpEnum';

import {getUserInfo, getUserInfo as getUserInfoApi, login} from '@/api/system/user';
import { storage } from '@/utils/Storage';
import Api from "@/api";
import {AccessTokenResponse} from "@/api/globals";

export type UserInfoType = {
  userName: string;
  name: string;
  email: string;
  phoneNumber: string;
};

export interface IUserState {
  token: string;
  username: string;
  welcome: string;
  avatar: string;
  permissions: any[];
  info: UserInfoType;
  refreshToken: string;
  tokenExpireTime: number;
}

export const useUserStore = defineStore({
  id: 'app-user',
  state: (): IUserState => ({
    token: storage.get(ACCESS_TOKEN, ''),
    username: '',
    welcome: '',
    avatar: '',
    permissions: [],
    info: storage.get(CURRENT_USER, {}),
    refreshToken: storage.get(REFRESH_TOKEN, ''),
    tokenExpireTime: storage.get(TOKEN_EXPIRE_TIME, 0),
  }),
  getters: {
    getToken(): string {
      return this.token;
    },
    getAvatar(): string {
      return this.avatar;
    },
    getNickname(): string {
      return this.username;
    },
    getPermissions(): [any][] {
      return this.permissions;
    },
    getUserInfo(): UserInfoType {
      return this.info;
    },
    getTokenExpireTime(): number {
      return this.tokenExpireTime;
    },
    getIsLoggedIn(): boolean {
      return typeof this.token === 'string' && this.token.length > 0;
    }
  },
  actions: {
    setToken(token: string) {
      this.token = token;
    },
    setAvatar(avatar: string) {
      this.avatar = avatar;
    },
    setPermissions(permissions) {
      this.permissions = permissions;
    },
    setUserInfo(info: UserInfoType) {
      this.info = info;
    },
    setRefreshToken(token: string) {
      this.refreshToken = token;
    },
    setTokenExpireTime(expireTime: number) {
      this.tokenExpireTime = expireTime;
    },
    // 登录
    async login(params: any) {
      const response = await login(params);
      this.setupToken(response);
      const userInfo = await getUserInfo();
      this.setUserInfo(userInfo as UserInfoType);
      
      return response;
    },

    // 获取用户信息
    async getInfo() {
      const data = await getUserInfoApi();
      // const { result } = data;
      // if (result.permissions && result.permissions.length) {
      //   const permissionsList = result.permissions;
      //   this.setPermissions(permissionsList);
      // } else {
      //   throw new Error('getInfo: permissionsList must be a non-null array !');
      // }
      // this.setAvatar(result.avatar);
      this.setUserInfo(data);
      return data;
    },

    // 登出
    async logout() {
      this.setPermissions([]);
      this.setUserInfo({ userName: '', email: '' });
      
      storage.remove(ACCESS_TOKEN);
      storage.remove(CURRENT_USER);
      storage.remove(REFRESH_TOKEN);
    },
    
    async refresh() {
      if (typeof this.refreshToken !== "string" || this.refreshToken.length === 0) {
        return;
      }
      
      const response = await Api.auth.post_api_v1_auth_refresh({
        data: {
          refreshToken: this.refreshToken,
        }
      });

      this.setupToken(response);
    },
    
    setupToken(response: AccessTokenResponse) {
      const ex = response.expiresIn ?? 0;
      const expireTime = Math.floor(Date.now() / 1000) + ex;
      console.log('token expires on', expireTime);
      storage.set(ACCESS_TOKEN, response.accessToken, ex);
      storage.set(REFRESH_TOKEN, response.refreshToken, 7 * 24 * 60 * 60);
      storage.set(TOKEN_EXPIRE_TIME, expireTime);
      this.setToken(response.accessToken ?? "");
      this.setRefreshToken(response.refreshToken ?? "");
      this.setTokenExpireTime(expireTime);
      setTimeout(() => this.refresh(), (ex - 120) * 1000);
    },
    
    async init() {
      try{
        const now = Date.now() / 1000;
        if (now >= this.getTokenExpireTime - 120) {
          await this.refresh();
        } else {
          setTimeout(() => this.refresh(), (this.getTokenExpireTime - now - 120) * 1000);
        }
      } catch (e) {
        console.error('无法刷新令牌', e);
      }
      
      if (this.getIsLoggedIn) {
        try {
          await this.getInfo();
        } catch (e) {
          console.error('获取用户信息失败', e);
        }
      }
    }
  },
});

// Need to be used outside the setup
export function useUser() {
  return useUserStore(store);
}
