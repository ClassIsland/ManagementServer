<template>
  <div class="dashboard d-flex flex-col gap-y-4">
    <!-- 统计卡片 -->
    <n-grid :cols="4" :x-gap="12" :y-gap="12">
      <n-gi>
        <n-card>
          <n-statistic label="在线客户端" :value="data.onlineCount">
            <template #prefix>
              <n-icon :size="20" color="#18a058"><WifiOutlined /></n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card>
          <n-statistic label="客户端总数" :value="data.totalCount">
            <template #prefix>
              <n-icon :size="20" color="#2080f0"><DesktopOutlined /></n-icon>
            </template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card>
          <n-statistic label="在线率" :value="onlineRate">
            <template #suffix>%</template>
          </n-statistic>
        </n-card>
      </n-gi>
      <n-gi>
        <n-card>
          <n-statistic label="策略覆盖率" :value="policyRate">
            <template #suffix>%</template>
          </n-statistic>
        </n-card>
      </n-gi>
    </n-grid>

    <!-- 图表区域 -->
    <n-grid :cols="2" :x-gap="12" :y-gap="12">
      <!-- 审计事件趋势 -->
      <n-gi>
        <n-card title="审计事件趋势（近7天）">
          <div ref="auditChartRef" style="height: 300px"></div>
        </n-card>
      </n-gi>

      <!-- 分组分布 -->
      <n-gi>
        <n-card title="客户端分组分布">
          <div ref="groupChartRef" style="height: 300px"></div>
        </n-card>
      </n-gi>
    </n-grid>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, onMounted, onUnmounted, nextTick } from 'vue';
import { NGrid, NGi, NCard, NStatistic, NIcon } from 'naive-ui';
import { WifiOutlined, DesktopOutlined } from '@vicons/antd';
import { getDashboardOverview, type DashboardOverview } from '@/api/dashboard/overview';
import * as echarts from 'echarts';

const data = reactive<DashboardOverview>({
  onlineCount: 0,
  totalCount: 0,
  groupDistribution: [],
  auditEventTrend: [],
  versionDistribution: [],
  policyCoverage: { totalClients: 0, coveredClients: 0, coverageRate: 0 },
});

const onlineRate = computed(() =>
  data.totalCount > 0 ? Math.round((data.onlineCount / data.totalCount) * 100) : 0
);
const policyRate = computed(() =>
  data.policyCoverage.totalClients > 0
    ? Math.round(data.policyCoverage.coverageRate * 100)
    : 0
);

const auditChartRef = ref<HTMLElement>();
const groupChartRef = ref<HTMLElement>();
let auditChart: echarts.ECharts | null = null;
let groupChart: echarts.ECharts | null = null;

function renderAuditChart() {
  if (!auditChartRef.value) return;
  if (!auditChart) auditChart = echarts.init(auditChartRef.value);
  auditChart.setOption({
    tooltip: { trigger: 'axis' },
    xAxis: { type: 'category', data: data.auditEventTrend.map((i) => i.date) },
    yAxis: { type: 'value', minInterval: 1 },
    series: [
      {
        type: 'line',
        data: data.auditEventTrend.map((i) => i.count),
        smooth: true,
        areaStyle: { opacity: 0.3 },
        itemStyle: { color: '#2080f0' },
      },
    ],
    grid: { left: 40, right: 20, top: 20, bottom: 30 },
  });
}

function renderGroupChart() {
  if (!groupChartRef.value) return;
  if (!groupChart) groupChart = echarts.init(groupChartRef.value);
  groupChart.setOption({
    tooltip: { trigger: 'item' },
    series: [
      {
        type: 'pie',
        radius: ['40%', '70%'],
        data: data.groupDistribution.map((g) => ({
          name: g.groupName,
          value: g.count,
        })),
        label: { show: true, formatter: '{b}: {c}' },
      },
    ],
  });
}

let refreshTimer: ReturnType<typeof setInterval> | null = null;

async function loadData() {
  try {
    const res = await getDashboardOverview();
    Object.assign(data, res);
    await nextTick();
    renderAuditChart();
    renderGroupChart();
  } catch (e) {
    console.error('加载看板数据失败', e);
  }
}

function handleResize() {
  auditChart?.resize();
  groupChart?.resize();
}

onMounted(() => {
  loadData();
  refreshTimer = setInterval(loadData, 30000);
  window.addEventListener('resize', handleResize);
});

onUnmounted(() => {
  if (refreshTimer) clearInterval(refreshTimer);
  window.removeEventListener('resize', handleResize);
  auditChart?.dispose();
  groupChart?.dispose();
});
</script>
