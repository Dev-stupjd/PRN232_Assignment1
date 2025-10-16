-- Test Data Script for FU News Management System
-- Run this after creating the database with Ass1.sql

USE [FUNewsManagement]
GO

-- Clear existing data (optional - remove if you want to keep existing data)
DELETE FROM [NewsTag]
DELETE FROM [NewsArticle]
DELETE FROM [SystemAccount]
DELETE FROM [Category]
DELETE FROM [Tag]
GO

-- Insert Categories
INSERT INTO [Category] ([CategoryName], [CategoryDesciption], [ParentCategoryID], [IsActive])
VALUES 
('Technology', 'Latest technology news and updates', NULL, 1),
('Sports', 'Sports news and events', NULL, 1),
('Politics', 'Political news and analysis', NULL, 1),
('Business', 'Business and economic news', NULL, 1),
('Entertainment', 'Entertainment industry news', NULL, 1),
('Mobile Technology', 'Mobile phones and apps news', 1, 1),
('AI & Machine Learning', 'Artificial Intelligence developments', 1, 1),
('Football', 'Football news and updates', 2, 1),
('Basketball', 'Basketball news and updates', 2, 1),
('Local Politics', 'Local government and politics', 3, 1)
GO

-- Insert System Accounts (Admin and Staff)
INSERT INTO [SystemAccount] ([AccountID], [AccountName], [AccountEmail], [AccountRole], [AccountPassword])
VALUES 
(1, 'Admin User', 'admin@funews.com', 1, 'admin123'),
(2, 'John Smith', 'john.smith@funews.com', 2, 'staff123'),
(3, 'Sarah Johnson', 'sarah.johnson@funews.com', 2, 'staff123'),
(4, 'Mike Wilson', 'mike.wilson@funews.com', 2, 'staff123'),
(5, 'Lisa Brown', 'lisa.brown@funews.com', 2, 'staff123'),
(6, 'David Lee', 'david.lee@funews.com', 2, 'staff123'),
(7, 'Emma Davis', 'emma.davis@funews.com', 2, 'staff123'),
(8, 'Tom Anderson', 'tom.anderson@funews.com', 2, 'staff123')
GO

-- Insert Tags
INSERT INTO [Tag] ([TagID], [TagName], [Note])
VALUES 
(1, 'Breaking News', 'Urgent and important news'),
(2, 'Exclusive', 'Exclusive content'),
(3, 'Analysis', 'In-depth analysis articles'),
(4, 'Interview', 'Interview-based articles'),
(5, 'Review', 'Product or service reviews'),
(6, 'Update', 'News updates'),
(7, 'Opinion', 'Opinion pieces'),
(8, 'Feature', 'Feature articles'),
(9, 'Tech Review', 'Technology product reviews'),
(10, 'Sports Update', 'Sports news updates')
GO

-- Insert News Articles
INSERT INTO [NewsArticle] ([NewsArticleID], [NewsTitle], [Headline], [CreatedDate], [NewsContent], [NewsSource], [CategoryID], [NewsStatus], [CreatedByID], [UpdatedByID], [ModifiedDate])
VALUES 
('ART20241201001', 'Revolutionary AI Technology Changes Everything', 'AI Breakthrough Announced', '2024-12-01 09:00:00', 'A new artificial intelligence system has been developed that promises to revolutionize multiple industries. The technology uses advanced machine learning algorithms to process data faster than ever before.', 'TechNews.com', 7, 1, 2, 2, '2024-12-01 10:30:00'),

('ART20241201002', 'Local Team Wins Championship', 'Victory in Final Match', '2024-12-01 14:30:00', 'The local football team secured their first championship in over a decade with a stunning victory in the final match. Fans celebrated throughout the city.', 'SportsDaily.com', 8, 1, 3, 3, '2024-12-01 16:00:00'),

('ART20241202001', 'New Economic Policy Announced', 'Government Reveals New Strategy', '2024-12-02 08:15:00', 'The government has announced a comprehensive new economic policy aimed at boosting growth and reducing unemployment. The policy includes tax incentives for small businesses.', 'PoliticsToday.com', 3, 1, 4, 4, '2024-12-02 09:45:00'),

('ART20241202002', 'Latest Smartphone Review', 'New Phone Impresses Critics', '2024-12-02 11:20:00', 'The latest smartphone from a major manufacturer has received rave reviews from technology critics. The device features an improved camera system and longer battery life.', 'MobileTech.com', 6, 1, 2, 2, '2024-12-02 12:30:00'),

('ART20241203001', 'Business Growth Continues', 'Companies Report Strong Q4', '2024-12-03 10:00:00', 'Major corporations have reported strong growth in the fourth quarter, with many exceeding analyst expectations. The business sector shows signs of continued expansion.', 'BusinessWeekly.com', 4, 1, 5, 5, '2024-12-03 11:15:00'),

('ART20241203002', 'Entertainment Industry Update', 'New Movies and Shows Announced', '2024-12-03 15:45:00', 'Several major entertainment companies have announced new movie and television show releases for the upcoming year. Fans can expect exciting new content.', 'EntertainmentNews.com', 5, 1, 6, 6, '2024-12-03 17:00:00'),

('ART20241204001', 'Basketball Season Begins', 'New Season Kicks Off', '2024-12-04 19:30:00', 'The professional basketball season has officially begun with exciting matches across the league. Teams are showing strong performance in early games.', 'SportsCentral.com', 9, 1, 3, 3, '2024-12-04 21:00:00'),

('ART20241204002', 'Local Government Meeting', 'City Council Discusses New Projects', '2024-12-04 18:00:00', 'The city council held a meeting to discuss several new infrastructure projects. Residents are encouraged to participate in upcoming public hearings.', 'LocalNews.com', 10, 1, 7, 7, '2024-12-04 19:30:00'),

('ART20241205001', 'Technology Conference Highlights', 'Innovation Showcased at Event', '2024-12-05 09:30:00', 'A major technology conference showcased the latest innovations in AI, robotics, and software development. Industry leaders shared insights about future trends.', 'TechConference.com', 1, 1, 2, 2, '2024-12-05 11:00:00'),

('ART20241205002', 'Inactive Article Test', 'This Article is Inactive', '2024-12-05 12:00:00', 'This article is set to inactive status for testing purposes. It should not appear in public news listings.', 'TestSource.com', 1, 0, 8, 8, '2024-12-05 13:00:00'),

-- Articles from different authors for testing author-specific queries
('ART20241206001', 'Sarah''s Tech Analysis', 'Deep Dive into AI Ethics', '2024-12-06 10:00:00', 'An in-depth analysis of the ethical implications of artificial intelligence development and deployment in modern society.', 'TechEthics.com', 7, 1, 3, 3, '2024-12-06 11:30:00'),

('ART20241206002', 'Mike''s Business Report', 'Market Analysis Q4', '2024-12-06 14:00:00', 'Comprehensive market analysis for the fourth quarter showing trends and predictions for the upcoming year.', 'MarketWatch.com', 4, 1, 4, 4, '2024-12-06 15:30:00'),

('ART20241207001', 'Lisa''s Entertainment Review', 'Movie Review: Latest Blockbuster', '2024-12-07 16:00:00', 'A detailed review of the latest blockbuster movie, covering plot, acting, and overall entertainment value.', 'MovieReviews.com', 5, 1, 5, 5, '2024-12-07 17:30:00'),

('ART20241208001', 'David''s Sports Commentary', 'Basketball Strategy Analysis', '2024-12-08 20:00:00', 'Analysis of basketball strategies used by top teams and how they might evolve in the coming season.', 'SportsStrategy.com', 9, 1, 6, 6, '2024-12-08 21:30:00'),

('ART20241209001', 'Emma''s Political Opinion', 'Local Politics Perspective', '2024-12-09 13:00:00', 'Opinion piece on recent developments in local politics and their impact on the community.', 'LocalOpinion.com', 10, 1, 7, 7, '2024-12-09 14:30:00'),

('ART20241210001', 'Tom''s Tech Review', 'Gadget Review: Latest Device', '2024-12-10 11:00:00', 'Comprehensive review of the latest technology gadget, covering features, performance, and value for money.', 'GadgetReview.com', 6, 1, 8, 8, '2024-12-10 12:30:00')
GO

-- Insert News-Tag relationships
INSERT INTO [NewsTag] ([NewsArticleID], [TagID])
VALUES 
-- Article 1 (AI Technology)
('ART20241201001', 1), -- Breaking News
('ART20241201001', 2), -- Exclusive
('ART20241201001', 3), -- Analysis

-- Article 2 (Football Championship)
('ART20241201002', 1), -- Breaking News
('ART20241201002', 6), -- Update
('ART20241201002', 10), -- Sports Update

-- Article 3 (Economic Policy)
('ART20241202001', 1), -- Breaking News
('ART20241202001', 3), -- Analysis

-- Article 4 (Smartphone Review)
('ART20241202002', 5), -- Review
('ART20241202002', 9), -- Tech Review

-- Article 5 (Business Growth)
('ART20241203001', 6), -- Update
('ART20241203001', 3), -- Analysis

-- Article 6 (Entertainment)
('ART20241203002', 6), -- Update
('ART20241203002', 8), -- Feature

-- Article 7 (Basketball Season)
('ART20241204001', 6), -- Update
('ART20241204001', 10), -- Sports Update

-- Article 8 (Local Government)
('ART20241204002', 6), -- Update

-- Article 9 (Tech Conference)
('ART20241205001', 2), -- Exclusive
('ART20241205001', 8), -- Feature

-- Article 10 (Inactive - no tags for testing)

-- Article 11 (Sarah's AI Ethics)
('ART20241206001', 3), -- Analysis
('ART20241206001', 7), -- Opinion

-- Article 12 (Mike's Business Report)
('ART20241206002', 3), -- Analysis
('ART20241206002', 6), -- Update

-- Article 13 (Lisa's Movie Review)
('ART20241207001', 5), -- Review
('ART20241207001', 7), -- Opinion

-- Article 14 (David's Basketball Analysis)
('ART20241208001', 3), -- Analysis
('ART20241208001', 10), -- Sports Update

-- Article 15 (Emma's Political Opinion)
('ART20241209001', 7), -- Opinion

-- Article 16 (Tom's Gadget Review)
('ART20241210001', 5), -- Review
('ART20241210001', 9)  -- Tech Review
GO

-- Verify data insertion
SELECT 'Categories' as TableName, COUNT(*) as RecordCount FROM [Category]
UNION ALL
SELECT 'SystemAccounts', COUNT(*) FROM [SystemAccount]
UNION ALL
SELECT 'Tags', COUNT(*) FROM [Tag]
UNION ALL
SELECT 'NewsArticles', COUNT(*) FROM [NewsArticle]
UNION ALL
SELECT 'NewsTags', COUNT(*) FROM [NewsTag]
GO

-- Show sample data for verification
SELECT 'Sample Categories:' as Info
SELECT CategoryID, CategoryName, CategoryDesciption FROM [Category] WHERE CategoryID <= 5

SELECT 'Sample Accounts:' as Info
SELECT AccountID, AccountName, AccountEmail, AccountRole FROM [SystemAccount] WHERE AccountID <= 5

SELECT 'Sample Articles:' as Info
SELECT NewsArticleID, NewsTitle, Headline, CreatedDate, NewsStatus, CreatedByID FROM [NewsArticle] WHERE NewsArticleID IN ('ART20241201001', 'ART20241201002', 'ART20241202001')

SELECT 'Sample Tags:' as Info
SELECT TagID, TagName, Note FROM [Tag] WHERE TagID <= 5
GO

PRINT 'Test data insertion completed successfully!'
PRINT 'You can now test the following scenarios:'
PRINT '1. Account deletion with articles (should fail)'
PRINT '2. Category deletion with articles (should fail)'
PRINT '3. Public news access (only active articles)'
PRINT '4. Author-specific article queries'
PRINT '5. Date range reports'
PRINT '6. Search functionality'
PRINT '7. Authentication with test accounts'

