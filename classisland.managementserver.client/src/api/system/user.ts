import { Alova } from '@/utils/http/alova';
import Apis from '@/api'

/**
 * @description: 获取用户信息
 */
export function getUserInfo() {
  return Apis.users.get_api_v1_users_current({});
}

/**
 * @description: 用户登录
 */
export function login(params) {
  return Apis.auth.post_api_v1_auth_login({
    params: {
    },
    data: {
      username: params.username,
      password: params.password,
    },
    meta: {
      ignoreAuth: true
    }
  })
}

/**
 * @description: 用户修改密码
 */
export function changePassword(params, uid) {
  return Alova.Post(`/user/u${uid}/changepw`, { params });
}

/**
 * @description: 用户登出
 */
export function logout(params) {
  
}
