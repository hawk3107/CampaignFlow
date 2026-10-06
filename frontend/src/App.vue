<script setup>
import { computed,onMounted, ref } from 'vue'

const campaigns = ref([])
const selectedCampaign = ref(null)
const loading = ref(true)

const totalBudget = computed(() =>
  campaigns.value.reduce((sum, campaign) => sum + campaign.budget, 0)
)

const totalSpend = computed(() =>
  campaigns.value.reduce((sum, campaign) => sum + campaign.spend, 0)
)

const totalImpressions = computed(() =>
  campaigns.value.reduce((sum, campaign) => sum + campaign.impressions, 0)
)

const totalClicks = computed(() =>
  campaigns.value.reduce((sum, campaign) => sum + campaign.clicks, 0)
)

const totalConversions = computed(() =>
  campaigns.value.reduce((sum, campaign) => sum + campaign.conversions, 0)
)

const totalCtr = computed(() => {
  return totalImpressions.value > 0
    ? ((totalClicks.value / totalImpressions.value) * 100).toFixed(2)
    : '0.00'
})

const totalConversionRate = computed(() => {
  return totalClicks.value > 0
    ? ((totalConversions.value / totalClicks.value) * 100).toFixed(2)
    : '0.00'
})

const filteredCampaigns = computed(() => {
  const query = searchQuery.value.toLowerCase().trim()

  return campaigns.value.filter(campaign => {
    const matchesSearch =
      campaign.name.toLowerCase().includes(query) ||
      campaign.advertiser.toLowerCase().includes(query) ||
      campaign.status.toLowerCase().includes(query)

    const matchesStatus =
      statusFilter.value === 'All' ||
      campaign.status === statusFilter.value

    return matchesSearch && matchesStatus
  })
})

const error = ref('')
const showCreateForm = ref(false)
const editingCampaign = ref(null)
const searchQuery = ref('')
const statusFilter = ref('All')

const API_URL = 'http://localhost:5064/api/campaigns'

async function loadCampaigns() {
  try {
    loading.value = true
    error.value = ''

    const response = await fetch(API_URL)

    if (!response.ok) {
      throw new Error('Failed to load campaigns')
    }

    campaigns.value = await response.json()

    if (campaigns.value.length > 0) {
      await loadAnalytics(campaigns.value[0].id)
    }
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function loadAnalytics(id) {
  try {
    const response = await fetch(`${API_URL}/${id}/analytics`)

    if (!response.ok) {
      throw new Error('Failed to load analytics')
    }

    selectedCampaign.value = await response.json()
  } catch (err) {
    error.value = err.message
  }
}

async function createCampaign(event) {
  event.preventDefault()

  const form = event.target

  const campaign = {
    name: form.name.value,
    advertiser: form.advertiser.value,
    budget: Number(form.budget.value),
    spend: Number(form.spend.value),
    impressions: Number(form.impressions.value),
    clicks: Number(form.clicks.value),
    conversions: Number(form.conversions.value),
    status: form.status.value,
    startDate: form.startDate.value,
    endDate: form.endDate.value
  }

  try {
  const isEditing = editingCampaign.value !== null

  const url = isEditing
    ? `${API_URL}/${editingCampaign.value.id}`
    : API_URL

  const method = isEditing ? 'PUT' : 'POST'

  const requestBody = isEditing
    ? {
        id: editingCampaign.value.id,
        ...campaign
      }
    : campaign

  const response = await fetch(url, {
    method: method,
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(requestBody)
  })

  if (!response.ok) {
    throw new Error(
      isEditing
        ? 'Failed to update campaign'
        : 'Failed to create campaign'
    )
  }

  showCreateForm.value = false
  editingCampaign.value = null

  await loadCampaigns()
  } catch (err) {
    error.value = err.message
  }
}

async function deleteCampaign(id) {
  const confirmed = confirm('Are you sure you want to delete this campaign?')

  if (!confirmed) return

  try {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE'
    })

    if (!response.ok) {
      throw new Error('Failed to delete campaign')
    }

    await loadCampaigns()
  } catch (err) {
    error.value = err.message
  }
}

function editCampaign(campaign) {
  editingCampaign.value = { ...campaign }
  showCreateForm.value = true
}

onMounted(loadCampaigns)
</script>

<template>
  <div class="app">
    <header class="topbar">
      <div class="brand">
        <div class="logo">CF</div>
        <div>
          <h1>CampaignFlow</h1>
          <p>Advertising Analytics Platform</p>
        </div>
      </div>

      <div class="status">
        <span class="status-dot"></span>
        API Connected
      </div>
    </header>

    <main class="container">
    <div v-if="showCreateForm" class="form-card">
  <div class="form-header">
    <div>
      <h3>
        {{ editingCampaign ? 'Edit Campaign' : 'Create New Campaign' }}
      </h3>
    <p>
      {{ editingCampaign
      ? 'Update the campaign details below.'
      : 'Enter campaign details to add a new advertising campaign.' }}
    </p>
    </div>

    <button
      class="close-btn"
      @click="showCreateForm = false; editingCampaign = null"
    >
      ×
    </button>
  </div>

  <form class="campaign-form" @submit="createCampaign">
    <div class="form-grid">

      <div class="form-group">
        <label>Campaign Name</label>
        <input 
          type="text" 
          name="name" 
          placeholder="e.g. Winter Sale Campaign"
          :value="editingCampaign?.name || ''"
        />
      </div>

      <div class="form-group">
        <label>Advertiser</label>
        <input
          type="text"
          name="advertiser"
          placeholder="e.g. Demo Advertiser"
          :value="editingCampaign?.advertiser ||''"
        />
      </div>

      <div class="form-group">
        <label>Budget</label>
        <input
          type="number"
          name="budget"
          placeholder="50000"
          :value="editingCampaign?.budget ||''"
        />
      </div>

      <div class="form-group">
        <label>Spend</label>
        <input
          type="number"
          name="spend"
          placeholder="10000"
          :value="editingCampaign?.spend ||''"
        />
      </div>

      <div class="form-group">
        <label>Impressions</label>
        <input
          type="number"
          name="impressions"
          placeholder="100000"
          :value="editingCampaign?.impressions ||''"
        />
      </div>

      <div class="form-group">
        <label>Clicks</label>
        <input
          type="number"
          name="clicks"
          placeholder="5000"
          :value="editingCampaign?.clicks ||''"
        />
      </div>

      <div class="form-group">
        <label>Conversions</label>
        <input
          type="number"
          name="conversions"
          placeholder="250"
          :value="editingCampaign?.conversions ||''"
        />
      </div>

      <div class="form-group">
        <label>Status</label>
        <select name="status" :value="editingCampaign?.status ||'Active'">
          <option>Active</option>
          <option>Paused</option>
          <option>Completed</option>
        </select>
      </div>

      <div class="form-group">
        <label>Start Date</label>
        <input type="date" 
        name="startDate"
        :value="editingCampaign?.startDate?.substring(0, 10) || ''"      
        />
      </div>

      <div class="form-group">
        <label>End Date</label>
        <input type="date"
        name="endDate" 
        :value="editingCampaign?.endDate?.substring(0, 10) || ''"
        />
      </div>

    </div>

    <div class="form-actions">
      <button
        type="button"
        class="cancel-btn"
        @click="showCreateForm = false;editingCampaign = null"
      >
        Cancel
      </button>

      <button type="submit" class="submit-btn">
        {{ editingCampaign ? 'Update Campaign' : 'Create Campaign' }}
      </button>
    </div>
  </form>
</div>
      <section class="page-header">
        <div>
          <h2>Campaign Dashboard</h2>
          <p>Monitor advertising campaign performance in real time.</p>
        </div>

        <div class="header-actions">
          <button class="create-btn" 
            @click="editingCampaign = null; showCreateForm = true"
          >
            + Create Campaign
          </button>

          <button class="refresh-btn" @click="loadCampaigns">
          Refresh
          </button>
        </div>
      </section>

      <div v-if="loading" class="message">
        Loading campaigns...
      </div>

      <div v-else-if="error" class="message error">
        {{ error }}
      </div>

      <template v-else>
        <section class="stats-grid">
          <div class="stat-card">
            <span class="stat-label">Total Campaigns</span>
            <strong>{{ campaigns.length }}</strong>
            <small>Active campaigns</small>
          </div>

          <div class="stat-card">
            <span class="stat-label">Total Budget</span>
            <strong>₹{{ totalBudget }}</strong>
            <small>Combined campaign budget</small>
          </div>

          <div class="stat-card">
            <span class="stat-label">Total Spend</span>
            <strong>₹{{ totalSpend }}</strong>
            <small>Total advertising spend</small>
          </div>

          <div class="stat-card">
            <span class="stat-label">Conversions</span>
            <strong>{{ totalConversions }}</strong>
            <small>Total campaign conversions</small>
          </div>
        </section>

        <section class="stats-grid">
  <div class="stat-card">
    <span class="stat-label">Impressions</span>
    <strong>{{ totalImpressions }}</strong>
    <small>Total ad impressions</small>
  </div>

  <div class="stat-card">
    <span class="stat-label">Clicks</span>
    <strong>{{ totalClicks }}</strong>
    <small>Total campaign clicks</small>
  </div>

  <div class="stat-card">
    <span class="stat-label">CTR</span>
    <strong>{{ totalCtr }}%</strong>
    <small>Click-through rate</small>
  </div>

  <div class="stat-card">
    <span class="stat-label">Conversion Rate</span>
    <strong>{{ totalConversionRate }}%</strong>
    <small>Conversions per click</small>
  </div>
</section>

        <section class="campaign-section">
      <div class="section-header">
  <div>
    <h3>Campaigns</h3>
    <span>{{ filteredCampaigns.length }} campaign(s)</span>
  </div>

  <div class="filters">
    <input
      v-model="searchQuery"
      type="text"
      class="search-input"
      placeholder="Search campaigns..."
    />

    <select v-model="statusFilter" class="status-filter">
      <option value="All">All Statuses</option>
      <option value="Active">Active</option>
      <option value="Paused">Paused</option>
      <option value="Completed">Completed</option>
    </select>
  </div>
</div>

          <div class="campaign-table">
            <div class="table-header">
              <span>Campaign</span>
              <span>Advertiser</span>
              <span>Budget</span>
              <span>Spend</span>
              <span>Status</span>
              <span>Actions</span>
            </div>

            <div
              v-for="campaign in filteredCampaigns"
              :key="campaign.id"
              class="table-row"
              @click="loadAnalytics(campaign.id)"
            >
              <span class="campaign-name">
                {{ campaign.name }}
              </span>

              <span>{{ campaign.advertiser }}</span>

              <span>₹{{ campaign.budget }}</span>

              <span>₹{{ campaign.spend }}</span>

              <span>
                <span class="badge">
                  {{ campaign.status }}
                </span>
              </span>
              <div class="actions">
                <button class="edit-btn" @click.stop="editCampaign(campaign)">
                  Edit
                </button>

                <button class="delete-btn" @click.stop="deleteCampaign(campaign.id)">
                  Delete
                </button>
              </div>
            </div>
          </div>
        </section>
      </template>
    </main>
  </div>
</template>

<style>
* {
  box-sizing: border-box;
}

body {
  margin: 0;
  font-family: Inter, Arial, sans-serif;
  background: #f5f7fb;
  color: #172033;
}

button {
  font-family: inherit;
}

.app {
  min-height: 100vh;
}

.topbar {
  height: 76px;
  background: #ffffff;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 42px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
}

.logo {
  width: 42px;
  height: 42px;
  border-radius: 10px;
  background: #111827;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
}

.brand h1 {
  margin: 0;
  font-size: 20px;
}

.brand p {
  margin: 3px 0 0;
  color: #6b7280;
  font-size: 12px;
}

.status {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #374151;
  font-size: 14px;
}

.status-dot {
  width: 9px;
  height: 9px;
  background: #22c55e;
  border-radius: 50%;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 42px;
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 30px;
}

.page-header h2 {
  margin: 0;
  font-size: 30px;
}

.page-header p {
  margin-top: 8px;
  color: #6b7280;
}

.refresh-btn {
  border: none;
  background: #111827;
  color: white;
  padding: 11px 20px;
  border-radius: 8px;
  cursor: pointer;
}

.header-actions {
  display: flex;
  gap: 10px;
  align-items: center;
}

.create-btn {
  border: none;
  background: #2563eb;
  color: white;
  padding: 11px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
}

.create-btn:hover {
  opacity: 0.9;
}

.refresh-btn:hover {
  opacity: 0.9;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
  margin-bottom: 30px;
}

.stat-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 22px;
}

.stat-label {
  display: block;
  color: #6b7280;
  font-size: 13px;
  margin-bottom: 10px;
}

.stat-card strong {
  display: block;
  font-size: 28px;
  margin-bottom: 5px;
}

.stat-card small {
  color: #9ca3af;
}

.campaign-section {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 22px;
  border-bottom: 1px solid #e5e7eb;
}

.section-header h3 {
  margin: 0;
  font-size: 18px;
}

.section-header span {
  color: #6b7280;
  font-size: 13px;
}

.campaign-table {
  overflow-x: auto;
  width: 100%;
}

.table-header,
.table-row {
  display: grid;
  grid-template-columns: 2fr 1.5fr 1fr 1fr 1fr 1.3fr;
  align-items: center;
  padding: 16px 22px;
}

.table-header {
  background: #f9fafb;
  color: #6b7280;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.table-row {
  border-top: 1px solid #f0f1f3;
  font-size: 14px;
  cursor: pointer;
}

.table-row:hover {
  background: #f9fafb;
}

.campaign-name {
  font-weight: 600;
}

.badge {
  display: inline-block;
  background: #dcfce7;
  color: #166534;
  padding: 5px 10px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 600;
}

.message {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 30px;
  text-align: center;
}

.error {
  color: #b91c1c;
}

@media (max-width: 900px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .container {
    padding: 25px;
  }
}

@media (max-width: 650px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }

  .topbar {
    padding: 0 20px;
  }

  .container {
    padding: 20px;
  }

  .table-header,
  .table-row {
  grid-template-columns: 180px 130px 110px 110px 110px 120px;
  min-width: 760px;
  padding: 12px 10px;
  font-size: 12px;
  }
  
}

.form-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 25px;
  margin-bottom: 30px;
}

.form-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 25px;
}

.form-header h3 {
  margin: 0;
  font-size: 20px;
}

.form-header p {
  margin: 6px 0 0;
  color: #6b7280;
  font-size: 13px;
}

.close-btn {
  border: none;
  background: transparent;
  font-size: 28px;
  cursor: pointer;
  color: #6b7280;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 18px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 7px;
}

.form-group label {
  font-size: 13px;
  font-weight: 600;
  color: #374151;
}

.form-group input,
.form-group select {
  padding: 11px 12px;
  border: 1px solid #d1d5db;
  border-radius: 7px;
  font-size: 14px;
  outline: none;
}

.form-group input:focus,
.form-group select:focus {
  border-color: #2563eb;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 25px;
}

.cancel-btn {
  border: 1px solid #d1d5db;
  background: white;
  padding: 11px 20px;
  border-radius: 8px;
  cursor: pointer;
}

.submit-btn {
  border: none;
  background: #2563eb;
  color: white;
  padding: 11px 20px;
  border-radius: 8px;
  cursor: pointer;
}

@media (max-width: 650px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}

.search-input {
  width: 260px;
  padding: 10px 14px;
  border: 1px solid #d9dee7;
  border-radius: 8px;
  font-size: 14px;
  outline: none;
}

.filters {
  display: flex;
  align-items: center;
  gap: 10px;
}

.status-filter {
  display: block;
  width: 160px;
  height: 40px;
  padding: 8px 12px;
  border: 1px solid #d9dee7;
  border-radius: 8px;
  font-size: 14px;
  background-color: white;
  color: #333;
  outline: none;
  cursor: pointer;
}

.status-filter:focus {
  border-color: #4f46e5;
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.search-input:focus {
  border-color: #4f46e5;
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

@media (max-width: 700px) {
  .section-header {
    flex-direction: column;
    align-items: stretch;
    gap: 14px;
  }

  .filters {
    width: 100%;
  }

  .search-input {
    flex: 1;
    width: auto;
  }

  .status-filter {
    width: 150px;
    flex-shrink: 0;
  }
}
</style>
