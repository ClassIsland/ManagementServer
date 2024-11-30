import { Alova } from '@/utils/http/alova';
import Apis from '@/api'

/**
 * @description: 获取用户信息
 */
export function getUserInfo() {
  return Apis.identity.get_api_v1_identity_manage_info({});
}

/**
 * @description: 用户登录
 */
export function login(params) {
  return Apis.identity.post_api_v1_identity_login({
    params: {
    },
    data: {
      email: params.username,
      password: params.password,
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
  return Alova.Post('/login/logout', {
    params,
  });
}
