import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import {ClusterOutlined, GroupOutlined} from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = "objectGroups"

const routes: Array<RouteRecordRaw> = [
  {
    path: '/groups',
    name: routeName,
    redirect: '/groups/list',
    component: Layout,
    meta: {
      title: '对象组',
      icon: renderIcon(GroupOutlined),
      sort: 97,
    },
    children: [
      {
        path: 'list',
        name: `${routeName}_groups`,
        meta: {
          title: '对象组',
          icon: renderIcon(GroupOutlined),
        },
        component: () => import('@/views/profiles/groups/index.vue'),
      },
    ]
  },
];

export default routes;
