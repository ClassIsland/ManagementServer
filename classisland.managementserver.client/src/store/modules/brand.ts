import { defineStore } from 'pinia'

export const useBrand = defineStore('brand', {
  state: () => ({
    organizationName: "",
    logoUrl: null,
    customLoginBanner: null,
    loginFormPlacement: "left"
  }),
  getters: {
    getOrganizationName: (state) => state.organizationName,
    getLogoUrl: (state) => state.logoUrl,
    getCustomLoginBanner: (state) => state.customLoginBanner,
    getLoginFormPlacement: (state) => state.loginFormPlacement
  },
  actions: {
    async init() {
      const result = await Apis.organizationsettings.get_api_v1_settings_brand();
      if (!result) {
        return;
      }
      this.organizationName = result.organizationName;
      this.logoUrl = result.logoUrl;
      this.customLoginBanner = result.customLoginBanner;
      this.loginFormPlacement = result.loginFormPlacement;
    }
  }
})
