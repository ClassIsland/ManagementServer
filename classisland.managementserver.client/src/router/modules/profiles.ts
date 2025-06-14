import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { FileOutlined, BookOutlined, TableOutlined, CalendarOutlined, UploadOutlined, GroupOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'profiles';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/profiles',
    name: routeName,
    component: Layout,
    meta: {
      title: '档案',
      icon: renderIcon(FileOutlined),
      sort: 1,
    },
    children: [
      {
        path: 'classplans',
        name: `${routeName}_classPlans`,
        meta: {
          title: '课表',
          icon: renderIcon(TableOutlined),
        },
        component: () => import('@/views/profiles/classplans/index.vue'),
      },
      {
        path: 'classplans/:id',
        name: `${routeName}_classPlans_editor`,
        meta: {
          title: '编辑课表',
          hidden: true,
          roles: ['ObjectsWrite']
        },
        component: () => import('@/views/profiles/classplans/editor.vue'),
      },
      {
        path: 'time_layouts',
        name: `${routeName}_time_layouts`,
        meta: {
          title: '时间表',
          icon: renderIcon(CalendarOutlined),
        },
        component: () => import('@/views/profiles/timeLayouts/index.vue'),
      },
      {
        path: 'time_layouts/:id',
        name: `${routeName}_time_layouts_editor`,
        meta: {
          title: '编辑时间表',
          hidden: true,
          roles: ['ObjectsWrite']
        },
        component: () => import('@/views/profiles/timeLayouts/editor.vue'),
      },
      {
        path: 'subjects',
        name: `${routeName}_subjects`,
        meta: {
          title: '科目',
          icon: renderIcon(BookOutlined),
        },
        component: () => import('@/views/profiles/subjects/index.vue'),
      },
      {
        path: 'upload',
        name: `${routeName}_upload`,
        meta: {
          title: '上传',
          icon: renderIcon(UploadOutlined),
          roles: ['ObjectsWrite']
        },
        component: () => import('@/views/profiles/upload/index.vue'),
      },
      
    ],
  },
];

export default routes;
