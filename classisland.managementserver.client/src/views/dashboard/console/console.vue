<template>
  <div class="console">
    <!--数据卡片-->
    <n-grid cols="1 s:2 m:3 l:4 xl:4 2xl:4" responsive="screen" :x-gap="12" :y-gap="8">

    </n-grid>

    <!--导航卡片-->


  </div>
</template>
<script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import { getConsoleInfo } from '@/api/dashboard/console';
  import VisiTab from './components/VisiTab.vue';
  import { CountTo } from '@/components/CountTo/index';
  import {
    CaretUpOutlined,
    CaretDownOutlined,
    UsergroupAddOutlined,
    BarChartOutlined,
    ShoppingCartOutlined,
    AccountBookOutlined,
    CreditCardOutlined,
    MailOutlined,
    TagsOutlined,
    SettingOutlined,
  } from '@vicons/antd';

  interface InVisits {
    dayVisits: number;
    rise: number;
    decline: number;
    amount: number;
  }

  interface InSaleroom {
    weekSaleroom: number;
    amount: number;
    degree: number;
  }

  interface InOrderLarge {
    weekLarge: number;
    rise: number;
    decline: number;
    amount: number;
  }

  interface InVolume {
    weekLarge: number;
    rise: number;
    decline: number;
    amount: number;
  }

  const loading = ref(true);
  const visits = ref({} as InVisits);
  const saleroom = ref({} as InSaleroom);
  const orderLarge = ref({} as InOrderLarge);
  const volume = ref({} as InVolume);

  // 图标列表
  const iconList = [
    {
      icon: UsergroupAddOutlined,
      size: '32',
      title: '用户',
      color: '#69c0ff',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: BarChartOutlined,
      size: '32',
      title: '分析',
      color: '#69c0ff',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: ShoppingCartOutlined,
      size: '32',
      title: '商品',
      color: '#ff9c6e',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: AccountBookOutlined,
      size: '32',
      title: '订单',
      color: '#b37feb',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: CreditCardOutlined,
      size: '32',
      title: '票据',
      color: '#ffd666',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: MailOutlined,
      size: '32',
      title: '消息',
      color: '#5cdbd3',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: TagsOutlined,
      size: '32',
      title: '标签',
      color: '#ff85c0',
      eventObject: {
        click: () => {},
      },
    },
    {
      icon: SettingOutlined,
      size: '32',
      title: '配置',
      color: '#ffc069',
      eventObject: {
        click: () => {},
      },
    },
  ];

  onMounted(async () => {
    const data = await getConsoleInfo();
    visits.value = data.visits;
    saleroom.value = data.saleroom;
    orderLarge.value = data.orderLarge;
    volume.value = data.volume;
    loading.value = false;
  });
  
</script>

<style lang="less" scoped></style>
