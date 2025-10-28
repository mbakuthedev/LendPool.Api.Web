# LendPool API - Product Requirements Document (PRD)
## Frontend Development Requirements

### Document Overview
This PRD outlines the comprehensive requirements for developing the frontend application for the LendPool API. The document covers all API endpoints, user roles, data structures, and frontend design considerations.

---

## 1. System Architecture Overview

### 1.1 User Roles
- **Borrower**: Users who request loans from lending pools
- **Lender**: Users who contribute to lending pools and earn returns
- **SuperLender**: Advanced lenders with pool management capabilities
- **Admin**: System administrators with full access

### 1.2 Authentication & Authorization
- JWT-based authentication with refresh tokens
- Role-based access control (RBAC)
- Protected routes based on user roles

---

## 2. Core Functional Areas

## 2.1 Authentication & User Management

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/auth/login` | Public | User login |
| POST | `/auth/register` | Public | User registration |
| POST | `/auth/refresh-token` | Public | Refresh JWT token |
| POST | `/auth/revoke-token` | Public | Revoke refresh token |
| GET | `/user/current` | Authenticated | Get current user details |
| GET | `/user/me` | Authenticated | Get user profile |
| POST | `/user/update-kyc-borrower` | Borrower | Update borrower KYC information |
| POST | `/user/kyc-lender` | Lender | Update lender KYC information |

### Frontend Requirements
- **Login/Register Forms**: Email, password, role selection
- **Profile Management**: User information display and editing
- **KYC Forms**: Separate forms for borrowers and lenders
- **Session Management**: Automatic token refresh, logout functionality
- **Role-based Navigation**: Different UI based on user role

---

## 2.2 Lender Pool Management

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/lender/create-pool` | Lender | Create new lending pool |
| GET | `/lender/get-pool-by-id` | Lender | Get pool details |
| GET | `/lender/get-all-pools` | Lender | Get all available pools |
| POST | `/lender/{poolId}/add-user` | SuperLender | Add user to pool |
| POST | `/lender/contribute/{poolId}` | Lender | Contribute to pool |
| POST | `/lender/withdraw/{poolId}` | Lender | Withdraw from pool |
| GET | `/lender/loans/{poolId}` | Lender | Get active loans for pool |
| GET | `/lender/summary/{poolId}` | Lender | Get pool summary |

### Frontend Requirements
- **Pool Creation Form**: Name, description, interest rate, min/max amounts
- **Pool Dashboard**: Overview of pool performance, member count, total funds
- **Pool Discovery**: Browse available pools with filters
- **Contribution Interface**: Amount input, confirmation dialogs
- **Pool Analytics**: Charts showing performance metrics
- **Member Management**: For SuperLenders to manage pool members

---

## 2.3 Pool Join Requests

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/pool/send-join-request` | Lender | Request to join a pool |
| GET | `/pool/get-join-requests` | SuperLender, Admin | Get join requests for pool |
| POST | `/pool/join-requests-{requestId}/review` | SuperLender, Admin | Accept/reject join request |

### Frontend Requirements
- **Join Request Form**: Pool selection, motivation/notes
- **Request Management Dashboard**: For SuperLenders to review requests
- **Request Status Tracking**: For lenders to see their request status
- **Bulk Actions**: Accept/reject multiple requests

---

## 2.4 Loan Management

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/loan/request-loan` | Borrower | Submit loan request |
| GET | `/loan/my-requests` | Borrower | Get user's loan requests |
| GET | `/loan/my-loans` | Borrower | Get user's active loans |
| GET | `/loan/get-loan-by-id` | Public | Get specific loan details |
| GET | `/loan/get-loans-by-pool-id` | Public | Get loans for specific pool |
| GET | `/borrower/get-all-pools` | Public | Get all pools for loan requests |
| PUT | `/loan/approve-loan` | Lender | Approve loan request |
| PUT | `/loan/reject-loan` | Lender | Reject loan request |

### Frontend Requirements
- **Loan Request Form**: Amount, purpose, duration, pool selection
- **Loan Dashboard**: Track loan status, payments, outstanding balance
- **Loan Approval Interface**: For lenders to review and approve/reject
- **Payment Schedule**: Visual timeline of payments
- **Loan History**: Past and current loans

---

## 2.5 Investment Management

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/api/lenderinvestment/create` | Lender, Admin | Create investment |
| POST | `/api/lenderinvestment/withdraw` | Lender, Admin | Withdraw investment |
| GET | `/api/lenderinvestment/investment/{investmentId}` | Lender, Admin | Get investment details |
| GET | `/api/lenderinvestment/pool/{poolId}` | Lender, Admin | Get investments by pool |
| GET | `/api/lenderinvestment/lender/{lenderId}` | Lender, Admin | Get investments by lender |
| GET | `/api/lenderinvestment/profit-share/{investmentId}` | Lender, Admin | Calculate profit share |
| GET | `/api/lenderinvestment/loss-share/{investmentId}` | Lender, Admin | Calculate loss share |
| GET | `/api/lenderinvestment/early-withdrawal-penalty/{investmentId}` | Lender, Admin | Calculate early withdrawal penalty |
| GET | `/api/lenderinvestment/pool-tenor/{poolId}` | Lender, Admin | Get pool tenor |
| PUT | `/api/lenderinvestment/pool-tenor/{poolId}/status` | Admin | Update pool tenor status |

### Frontend Requirements
- **Investment Dashboard**: Portfolio overview, performance metrics
- **Investment Calculator**: ROI, profit/loss projections
- **Withdrawal Interface**: Amount selection, penalty calculations
- **Investment History**: Track all investment activities
- **Performance Charts**: Visual representation of returns

---

## 2.6 Wallet & Transactions

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| GET | `/wallet/me` | Authenticated | Get wallet details |
| GET | `/wallet/balance` | Authenticated | Get wallet balance |
| POST | `/wallet/credit` | Authenticated | Credit wallet |
| POST | `/wallet/debit` | Authenticated | Debit wallet |
| POST | `/wallet/transfer` | Authenticated | Transfer between users |
| POST | `/wallet/create` | Authenticated | Create new wallet |
| GET | `/wallet/transactions` | Authenticated | Get transaction history |
| GET | `/wallet/transactions/{transactionId}` | Authenticated | Get specific transaction |

### Frontend Requirements
- **Wallet Dashboard**: Balance display, quick actions
- **Transaction History**: Filterable list with search
- **Transfer Interface**: Recipient selection, amount input, confirmation
- **Transaction Details**: Full transaction information
- **Balance Alerts**: Low balance warnings

---

## 2.7 Disbursement & Fund Usage

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/disbursement/create-disbursement` | Lender, Admin | Create disbursement |
| GET | `/disbursement/by-loan` | Lender, Borrower, Admin | Get disbursements by loan |
| GET | `/disbursement/get-by-id` | Lender, Borrower, Admin | Get disbursement details |
| POST | `/fundusage/log-usage` | Borrower, Admin | Log fund usage |
| GET | `/fundusage/usages` | Lender, Borrower, Admin | Get fund usage records |

### Frontend Requirements
- **Disbursement Tracking**: Visual timeline of fund releases
- **Fund Usage Logging**: Receipt upload, categorization
- **Usage Analytics**: Spending patterns, category breakdowns
- **Approval Workflow**: For fund usage verification

---

## 2.8 Reconciliation System

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/reconciliation/request` | Lender | Create reconciliation request |
| GET | `/reconciliation/{reconciliationId}` | Lender | Get reconciliation details |
| GET | `/reconciliation/loan/{loanId}` | Lender | Get reconciliations by loan |
| GET | `/reconciliation/lender/my-reconciliations` | Lender | Get lender's reconciliations |
| PUT | `/reconciliation/{reconciliationId}/status` | Lender | Update reconciliation status |
| PUT | `/reconciliation/item/{itemId}` | Lender | Update reconciliation item |
| POST | `/reconciliation/verify-fund-usage/{fundUsageId}` | Lender | Verify fund usage |
| GET | `/reconciliation/summary/{loanId}` | Lender | Get reconciliation summary |
| POST | `/reconciliation/{reconciliationId}/complete` | Lender | Complete reconciliation |
| GET | `/reconciliation/pool/{poolId}/summaries` | Lender | Get reconciliation summaries by pool |
| GET | `/reconciliation/{reconciliationId}/discrepancy` | Lender | Calculate discrepancy |
| PUT | `/reconciliation/{reconciliationId}/compliance` | Lender | Mark as compliant |

### Borrower Reconciliation Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| GET | `/borrower/reconciliation/my-reconciliations` | Borrower | Get borrower's reconciliations |
| GET | `/borrower/reconciliation/{reconciliationId}` | Borrower | Get reconciliation details |
| PUT | `/borrower/reconciliation/item/{itemId}/respond` | Borrower | Respond to reconciliation item |
| POST | `/borrower/reconciliation/verify-fund-usage/{fundUsageId}` | Borrower | Verify fund usage |
| GET | `/borrower/reconciliation/summary/{loanId}` | Borrower | Get reconciliation summary |

### Frontend Requirements
- **Reconciliation Dashboard**: Overview of all reconciliation activities
- **Discrepancy Resolution**: Interface to resolve discrepancies
- **Document Upload**: Receipt and supporting document upload
- **Approval Workflow**: Multi-step approval process
- **Compliance Tracking**: Visual compliance status indicators
- **Audit Trail**: Complete history of reconciliation activities

---

## 2.9 Voting System

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/api/voting/cast-vote` | Lender | Cast vote on operation |
| GET | `/api/voting/vote-result/{operationId}/{operationType}` | Lender | Get vote results |
| GET | `/api/voting/pool-votes/{poolId}` | Lender | Get pool vote results |
| GET | `/api/voting/pool-summary/{poolId}` | Lender | Get pool voting summary |
| GET | `/api/voting/has-voted/{operationId}/{operationType}` | Lender | Check if user has voted |
| GET | `/api/voting/pool-member-count/{poolId}` | Lender | Get pool member count |
| GET | `/api/voting/active-voter-count/{operationId}/{operationType}` | Lender | Get active voter count |

### Frontend Requirements
- **Voting Interface**: Clear vote options (Approve/Reject/Abstain)
- **Vote Results Display**: Real-time vote counting and percentages
- **Voting History**: Track past voting activities
- **Pool Voting Dashboard**: Overview of all pool voting activities
- **Vote Notifications**: Alerts for new voting opportunities

---

## 2.10 Lender Dashboard

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| GET | `/pool/loans/funded` | Lender | Get funded loans |
| GET | `/pool/repayments/earnings` | Lender | Get repayments and earnings |
| GET | `/pool/performance` | Lender | Get pool performance |

### Frontend Requirements
- **Dashboard Overview**: Key metrics, charts, and KPIs
- **Performance Analytics**: ROI, profit/loss tracking
- **Earnings Visualization**: Charts showing earnings over time
- **Loan Portfolio**: Visual representation of funded loans

---

## 2.11 Borrower Analytics

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| GET | `/borrower/analytics/total-borrowed` | Borrower | Get total borrowed amount |
| GET | `/borrower/analytics/current-loan` | Borrower | Get current loan details |
| GET | `/borrower/analytics/outstanding-balance` | Borrower | Get outstanding balance |
| GET | `/borrower/analytics/next-payment` | Borrower | Get next payment date |
| GET | `/borrower/analytics/loan-by-month` | Borrower | Get loan data by month |
| GET | `/borrower/analytics/repayments-by-month` | Borrower | Get repayment data by month |
| GET | `/borrower/analytics/repayment-history` | Borrower | Get repayment history |

### Frontend Requirements
- **Analytics Dashboard**: Comprehensive borrower analytics
- **Payment Calendar**: Visual payment schedule
- **Financial Charts**: Borrowing patterns, repayment trends
- **Credit Score Visualization**: If applicable
- **Payment Reminders**: Upcoming payment notifications

---

## 2.12 Admin Functions

### Endpoints
| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| POST | `/admin/credit` | Admin | Admin credit to user wallet |
| POST | `/admin/debit` | Admin | Admin debit from user wallet |
| GET | `/admin/loan/unmatched` | Admin | Get unmatched loan requests |
| GET | `/admin/loan/pools` | Admin | Get lender pools |
| POST | `/admin/loan/{loanRequestId}/assign-pool` | Admin | Assign pool to loan request |

### Frontend Requirements
- **Admin Dashboard**: System overview, user management
- **User Management**: Create, edit, disable users
- **Financial Controls**: Manual credit/debit operations
- **Loan Matching**: Interface to match loans with pools
- **System Analytics**: Platform-wide metrics and insights
- **Audit Logs**: Complete system activity logs

---

## 3. Data Models & Structures

### 3.1 Core Entities
- **User**: ID, Email, FullName, Role, KYC status
- **LenderPool**: ID, Name, Description, InterestRate, MinAmount, MaxAmount
- **Loan**: ID, Amount, InterestRate, StartDate, DueDate, Status
- **Investment**: ID, LenderId, PoolId, Amount, ProfitShare
- **Wallet**: ID, UserId, Balance, Transactions
- **Vote**: ID, LenderId, OperationId, VoteType, Comment

### 3.2 Enums
- **UserRole**: Borrower, Lender, SuperLender, Admin
- **LoanStatus**: Pending, Approved, Active, Completed, Defaulted
- **VoteType**: Approve, Reject, Abstain
- **TransactionType**: Credit, Debit, Transfer, Investment, Withdrawal

---

## 4. Frontend Technical Requirements

### 4.1 Technology Stack Recommendations
- **Framework**: React.js with TypeScript or Vue.js with TypeScript
- **State Management**: Redux Toolkit or Vuex
- **UI Library**: Material-UI, Ant Design, or Vuetify
- **Charts**: Chart.js, D3.js, or Recharts
- **HTTP Client**: Axios with interceptors for token management
- **Form Handling**: React Hook Form or VeeValidate
- **Routing**: React Router or Vue Router

### 4.2 Key Features
- **Responsive Design**: Mobile-first approach
- **Real-time Updates**: WebSocket integration for live data
- **Offline Support**: Service workers for critical functions
- **Progressive Web App**: PWA capabilities
- **Internationalization**: Multi-language support
- **Accessibility**: WCAG 2.1 compliance

### 4.3 Security Considerations
- **Token Storage**: Secure token handling (httpOnly cookies recommended)
- **Input Validation**: Client-side validation with server-side verification
- **XSS Protection**: Content Security Policy implementation
- **CSRF Protection**: Anti-CSRF token implementation
- **Role-based UI**: Conditional rendering based on user roles

---

## 5. User Experience Requirements

### 5.1 Dashboard Design
- **Role-specific Dashboards**: Different layouts for each user type
- **Widget-based Layout**: Customizable dashboard widgets
- **Quick Actions**: Prominent action buttons for common tasks
- **Notifications**: Real-time notification system
- **Search Functionality**: Global search across all data

### 5.2 Navigation Structure
- **Sidebar Navigation**: Collapsible sidebar with role-based menu items
- **Breadcrumb Navigation**: Clear navigation hierarchy
- **Tab Navigation**: For related functionality grouping
- **Mobile Navigation**: Hamburger menu with touch-friendly design

### 5.3 Data Visualization
- **Charts & Graphs**: Financial performance, trends, analytics
- **Data Tables**: Sortable, filterable, paginated tables
- **Progress Indicators**: Visual progress for multi-step processes
- **Status Indicators**: Clear visual status representations

---

## 6. Integration Requirements

### 6.1 API Integration
- **Base URL**: Configure API base URL for different environments
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Loading States**: Loading indicators for all async operations
- **Retry Logic**: Automatic retry for failed requests
- **Request/Response Logging**: Development debugging support

### 6.2 External Integrations
- **Payment Gateways**: Integration with payment processors
- **KYC Services**: Third-party identity verification
- **Notification Services**: Email, SMS, push notifications
- **File Upload**: Document and receipt upload functionality
- **Analytics**: User behavior tracking and analytics

---

## 7. Performance Requirements

### 7.1 Loading Performance
- **Initial Load**: < 3 seconds for initial page load
- **API Response**: < 2 seconds for standard API calls
- **Image Optimization**: Lazy loading and image compression
- **Code Splitting**: Route-based code splitting
- **Caching Strategy**: Browser caching and service worker caching

### 7.2 Scalability
- **Lazy Loading**: Component and route lazy loading
- **Virtual Scrolling**: For large data lists
- **Pagination**: Efficient data pagination
- **Debouncing**: Search and input debouncing
- **Memoization**: React.memo or Vue computed properties

---

## 8. Testing Requirements

### 8.1 Testing Strategy
- **Unit Tests**: Component and utility function testing
- **Integration Tests**: API integration testing
- **E2E Tests**: Critical user journey testing
- **Visual Regression Tests**: UI consistency testing
- **Performance Tests**: Load and performance testing

### 8.2 Test Coverage
- **Minimum 80% code coverage** for critical business logic
- **100% coverage** for authentication and authorization
- **API mocking** for development and testing environments
- **Cross-browser testing** for major browsers

---

## 9. Deployment & DevOps

### 9.1 Environment Configuration
- **Development**: Local development environment
- **Staging**: Pre-production testing environment
- **Production**: Live production environment
- **Environment Variables**: Secure configuration management

### 9.2 CI/CD Pipeline
- **Automated Testing**: Run tests on every commit
- **Build Automation**: Automated build and deployment
- **Code Quality**: ESLint, Prettier, SonarQube integration
- **Security Scanning**: Dependency vulnerability scanning

---

## 10. Success Metrics

### 10.1 User Experience Metrics
- **Page Load Time**: < 3 seconds
- **User Engagement**: Time spent on platform
- **Task Completion Rate**: Successful completion of user tasks
- **Error Rate**: < 1% error rate for user actions

### 10.2 Business Metrics
- **User Adoption**: New user registration rate
- **Feature Usage**: Usage statistics for key features
- **Conversion Rate**: User journey completion rates
- **Support Tickets**: Reduction in support requests

---

## 11. Future Enhancements

### 11.1 Planned Features
- **Mobile App**: Native iOS and Android applications
- **Advanced Analytics**: AI-powered insights and recommendations
- **Social Features**: User profiles, ratings, reviews
- **API Marketplace**: Third-party integrations
- **White-label Solution**: Customizable branding options

### 11.2 Technical Improvements
- **Microservices**: API microservices architecture
- **GraphQL**: Alternative to REST API
- **Real-time Features**: WebSocket implementation
- **Blockchain Integration**: Smart contract integration
- **Machine Learning**: Predictive analytics and fraud detection

---

## Conclusion

This PRD provides a comprehensive overview of the LendPool API requirements for frontend development. The document should be used as a reference throughout the development process to ensure all requirements are met and the final product delivers an excellent user experience across all user roles and use cases.

Regular updates to this document are recommended as the API evolves and new requirements are identified during the development process.
