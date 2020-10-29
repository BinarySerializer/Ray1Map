namespace R1Engine
{
    public static class GBARRR_Tables
    {
        // TODO: Correct anim speeds
        public static AnimBlock[][] GetAnimBlocks => new AnimBlock[][]
        {
            // HUD
            new AnimBlock[]
            {
                new AnimBlock(7, 23), // Font
                new AnimBlock(18, 17), // Timer bar
                new AnimBlock(25, 320), // Cage icon
                new AnimBlock(26, 23),
                new AnimBlock(28, 27),
                new AnimBlock(29, 23),
                new AnimBlock(30, 905, subPalette: 3),
                new AnimBlock(31, 905, subPalette: 3),
                new AnimBlock(32, 589),
                new AnimBlock(33, 405),
                new AnimBlock(34, 412),
                new AnimBlock(36, 35),
                new AnimBlock(38, 37),
                new AnimBlock(39, 642),
                new AnimBlock(40, 648),
                new AnimBlock(41, 644),
                new AnimBlock(43, 42),
                new AnimBlock(44, 42),
                new AnimBlock(46, 45),
                new AnimBlock(48, 47),
                new AnimBlock(50, 49),
                new AnimBlock(57, 56),
                //new AnimBlock(59, 58),
                new AnimBlock(61, 60),
                new AnimBlock(63, 62),
                new AnimBlock(65, 64),
                new AnimBlock(67, 66),
            },

            // Rayman
            new AnimBlock[]
            {
                new AnimBlock(145, 144),
                new AnimBlock(146, 144),
                new AnimBlock(147, 144),
                new AnimBlock(148, 144),
                new AnimBlock(149, 144),
                new AnimBlock(150, 144),
                new AnimBlock(151, 144),
                new AnimBlock(152, 144),
                new AnimBlock(153, 144),
                new AnimBlock(154, 144),
                new AnimBlock(155, 144),
                new AnimBlock(156, 144),
                new AnimBlock(157, 144),
                new AnimBlock(158, 144),
                new AnimBlock(159, 144),
                new AnimBlock(160, 144),
                new AnimBlock(161, 144),
                new AnimBlock(162, 144),
                new AnimBlock(163, 144),
                new AnimBlock(164, 144),
                new AnimBlock(165, 144),
                new AnimBlock(166, 144),
                new AnimBlock(167, 144),
                new AnimBlock(168, 144),
                new AnimBlock(169, 144),
                new AnimBlock(170, 144),
                new AnimBlock(171, 144),
                new AnimBlock(172, 144),
                new AnimBlock(173, 144),
                new AnimBlock(174, 144),
                new AnimBlock(175, 144),
                new AnimBlock(176, 144),
                new AnimBlock(177, 144),
                new AnimBlock(178, 144),
                new AnimBlock(179, 144),
                new AnimBlock(180, 144),
                new AnimBlock(181, 144),
                new AnimBlock(182, 144),
                new AnimBlock(183, 144),
                new AnimBlock(184, 144),
                new AnimBlock(185, 144),
                new AnimBlock(186, 144),
                new AnimBlock(187, 144),
                new AnimBlock(188, 144),
                new AnimBlock(189, 144),
                new AnimBlock(190, 144),
                new AnimBlock(191, 144),
                new AnimBlock(192, 144),
                new AnimBlock(193, 144),
                new AnimBlock(194, 144),
                new AnimBlock(195, 144),
                new AnimBlock(196, 144),
                new AnimBlock(197, 144),
                new AnimBlock(198, 144),
                new AnimBlock(199, 144),
                new AnimBlock(200, 144),
                new AnimBlock(201, 144),
                new AnimBlock(202, 144),
                new AnimBlock(203, 144),
                new AnimBlock(204, 144),
                new AnimBlock(205, 144),
                new AnimBlock(206, 144),
                new AnimBlock(207, 144),
                new AnimBlock(208, 144),
                new AnimBlock(209, 144),
                new AnimBlock(210, 144),
                new AnimBlock(211, 144),
                new AnimBlock(212, 144),
                new AnimBlock(213, 144),
                new AnimBlock(214, 144),
                new AnimBlock(215, 144),
                new AnimBlock(216, 144),
                new AnimBlock(217, 144),
                new AnimBlock(218, 144),
                new AnimBlock(219, 144),
                new AnimBlock(220, 144),
                new AnimBlock(221, 144),
                new AnimBlock(222, 144),
                new AnimBlock(223, 144),
                new AnimBlock(224, 144),
                new AnimBlock(225, 144),
                new AnimBlock(226, 144),
                new AnimBlock(227, 144),
                new AnimBlock(228, 144),
                new AnimBlock(229, 144),
                new AnimBlock(230, 144),
                new AnimBlock(232, 231),
                new AnimBlock(233, 231),
                new AnimBlock(234, 231),
                new AnimBlock(235, 231),
                new AnimBlock(236, 231),
                new AnimBlock(237, 231),
                new AnimBlock(238, 231),
                new AnimBlock(239, 231),
                new AnimBlock(240, 231),
                new AnimBlock(241, 231),
                new AnimBlock(242, 231),
                new AnimBlock(243, 231),
                new AnimBlock(244, 231),
                new AnimBlock(245, 231),
                new AnimBlock(246, 231),
                new AnimBlock(248, 247),
                new AnimBlock(249, 247),
                new AnimBlock(250, 247),
                new AnimBlock(251, 247),
                new AnimBlock(252, 247),
                new AnimBlock(253, 247),
                new AnimBlock(254, 247),
                new AnimBlock(255, 247),
                new AnimBlock(256, 247),
                new AnimBlock(257, 247),
                new AnimBlock(258, 247),
                new AnimBlock(259, 247),
                new AnimBlock(260, 247),
                new AnimBlock(261, 247),
                new AnimBlock(262, 247),
                new AnimBlock(263, 247),
                new AnimBlock(265, 264),
                new AnimBlock(266, 264),
                new AnimBlock(267, 264),
                new AnimBlock(268, 264),
                new AnimBlock(269, 264),
                new AnimBlock(270, 264),
                new AnimBlock(271, 264),
                new AnimBlock(272, 264),
                new AnimBlock(273, 264),
                new AnimBlock(274, 264),
                new AnimBlock(275, 264),
                new AnimBlock(276, 264),
                new AnimBlock(277, 264),
                new AnimBlock(278, 264),
                new AnimBlock(280, 279),
                new AnimBlock(281, 279),
                new AnimBlock(283, 282),
                new AnimBlock(284, 282),
                new AnimBlock(285, 282),
                new AnimBlock(286, 282),
                new AnimBlock(287, 282),
                new AnimBlock(288, 282),
                new AnimBlock(289, 282),
                new AnimBlock(290, 282),
                new AnimBlock(291, 282),
                new AnimBlock(292, 282),
                new AnimBlock(293, 282),
                new AnimBlock(294, 282),
                new AnimBlock(295, 282),
                new AnimBlock(297, 296),
                new AnimBlock(298, 296),
                new AnimBlock(299, 296),
                new AnimBlock(300, 296),
                new AnimBlock(301, 296),
                new AnimBlock(302, 296),
                new AnimBlock(303, 296),
                new AnimBlock(304, 296),
                new AnimBlock(305, 296),
                new AnimBlock(306, 296),
                new AnimBlock(307, 296),
                new AnimBlock(308, 296),
                new AnimBlock(309, 296),
                new AnimBlock(310, 296),
                new AnimBlock(311, 296),

                new AnimBlock(791, 790),
                new AnimBlock(792, 790),
                new AnimBlock(793, 790),
                new AnimBlock(794, 790),
                new AnimBlock(795, 790),
                new AnimBlock(796, 790),

                new AnimBlock(798, 797),
                new AnimBlock(800, 799),
                new AnimBlock(802, 801),
                new AnimBlock(804, 803),
                new AnimBlock(805, 803),
                new AnimBlock(806, 803),
                new AnimBlock(807, 803),
            },

            // Collectibles
            new AnimBlock[]
            {
                new AnimBlock(312, 320),
                new AnimBlock(313, 320),
                new AnimBlock(315, 314),
                new AnimBlock(317, 316),
                new AnimBlock(318, 316),
                new AnimBlock(319, 320),
                new AnimBlock(321, 320),
                new AnimBlock(322, 320),
                new AnimBlock(323, 320),
                new AnimBlock(324, 320),
                new AnimBlock(325, 320),
                new AnimBlock(326, 320),
                new AnimBlock(327, 320),
            }, 

            // Throwable objects
            new AnimBlock[]
            {
                new AnimBlock(329, 328),
                new AnimBlock(331, 330),
                new AnimBlock(333, 332, subPalette: 3),
                new AnimBlock(335, 334),
                new AnimBlock(337, 336),
                new AnimBlock(339, 338),
                new AnimBlock(341, 340),
                new AnimBlock(347, 346),
                new AnimBlock(690, 689),
            }, 

            // Drops
            new AnimBlock[]
            {
                new AnimBlock(343, 342),
                new AnimBlock(345, 344),
            },

            // Menu large Rabbid
            new AnimBlock[]
            {
                new AnimBlock(349, 348),
                new AnimBlock(351, 350),
                new AnimBlock(353, 352),
                new AnimBlock(355, 354),
                new AnimBlock(357, 356),
                new AnimBlock(359, 358),
            },

            // Press Start
            new AnimBlock[]
            {
                new AnimBlock(361, 360),
                new AnimBlock(362, 360),
                new AnimBlock(363, 360),
                new AnimBlock(364, 360),
                new AnimBlock(365, 360),
                new AnimBlock(366, 360),
            }, 

            // Menu small Rabbids
            new AnimBlock[]
            {
                new AnimBlock(368, 367),
                new AnimBlock(369, 367),
                new AnimBlock(370, 367),
            }, 

            // Stompable pencil
            new AnimBlock[]
            {
                new AnimBlock(372, 371),
            },

            // Moving platforms
            new AnimBlock[]
            {
                new AnimBlock(374, 373),
                new AnimBlock(376, 375),
                new AnimBlock(380, 379),
                new AnimBlock(391, 390),
                new AnimBlock(395, 394),
                new AnimBlock(397, 396),
            }, 

            // Bouncy platform
            new AnimBlock[]
            {
                new AnimBlock(378, 377),
                new AnimBlock(382, 381),
                new AnimBlock(393, 392),
                new AnimBlock(399, 398),
                new AnimBlock(401, 400),
                new AnimBlock(403, 402),
                new AnimBlock(404, 402),
            },

            // Ring
            new AnimBlock[]
            {
                new AnimBlock(384, 383),
                new AnimBlock(386, 385),
            }, 

            // Unused skull
            new AnimBlock[]
            {
                new AnimBlock(418, 412),
            }, 

            // Swinging fruit
            new AnimBlock[]
            {
                new AnimBlock(387, 379),
                new AnimBlock(389, 388),
            }, 

            // Murfy
            new AnimBlock[]
            {
                new AnimBlock(406, 405),
                new AnimBlock(407, 405),
                new AnimBlock(408, 405),
                new AnimBlock(409, 405),
                new AnimBlock(410, 405),
                new AnimBlock(411, 405),
            }, 

            // Ly
            new AnimBlock[]
            {
                new AnimBlock(413, 412),
                new AnimBlock(414, 412),
                new AnimBlock(415, 412),
                new AnimBlock(416, 412),
                new AnimBlock(417, 412),
            }, 

            // Enemy book
            new AnimBlock[]
            {
                new AnimBlock(420, 419),
                new AnimBlock(421, 419),
            }, 

            // Enemy backpack
            new AnimBlock[]
            {
                new AnimBlock(423, 422),
                new AnimBlock(424, 422),
                new AnimBlock(425, 422),
                new AnimBlock(426, 422),
                new AnimBlock(427, 422),
                new AnimBlock(429, 428),
            }, 

            // Enemy toy robot
            new AnimBlock[]
            {
                new AnimBlock(431, 430),
                new AnimBlock(432, 430),
                new AnimBlock(434, 433),
                new AnimBlock(435, 430),
                new AnimBlock(436, 430),
                new AnimBlock(437, 430),
            }, 

            // Superhero Rabbid
            new AnimBlock[]
            {
                new AnimBlock(439, 438),
                new AnimBlock(440, 438),
                new AnimBlock(441, 438),
                new AnimBlock(442, 438),
                new AnimBlock(443, 438),
                new AnimBlock(444, 438),
                new AnimBlock(445, 438),
            }, 

            // Dancing Rabbid
            new AnimBlock[]
            {
                new AnimBlock(447, 446),
                new AnimBlock(448, 446),
                new AnimBlock(449, 446),
                new AnimBlock(450, 446),
                new AnimBlock(451, 446),
                new AnimBlock(452, 446),
                new AnimBlock(453, 446),
            }, 

            // Enemy pen
            new AnimBlock[]
            {
                new AnimBlock(455, 454),
                new AnimBlock(456, 454),
                new AnimBlock(457, 454),
                new AnimBlock(458, 454),
                new AnimBlock(459, 454),
                new AnimBlock(460, 454),
                new AnimBlock(461, 454),
            }, 

            // Gray Rabbid
            new AnimBlock[]
            {
                new AnimBlock(463, 462),
                new AnimBlock(464, 462),
                new AnimBlock(465, 462),
                new AnimBlock(466, 462),
                new AnimBlock(467, 462),
                new AnimBlock(468, 462),
            }, 

            // Enemy larva
            new AnimBlock[]
            {
                new AnimBlock(470, 469),
                new AnimBlock(471, 469),
                new AnimBlock(472, 469),
                new AnimBlock(473, 469),
                new AnimBlock(474, 469),
                new AnimBlock(475, 469),
            }, 

            // Hunter
            new AnimBlock[]
            {
                new AnimBlock(477, 476),
                new AnimBlock(478, 476),
                new AnimBlock(479, 476),
                new AnimBlock(480, 476),
                new AnimBlock(481, 476),
                new AnimBlock(482, 476),
                new AnimBlock(483, 476),
                new AnimBlock(484, 476),
                new AnimBlock(485, 476),
                new AnimBlock(487, 486),
            }, 

            // Small lividstone
            new AnimBlock[]
            {
                new AnimBlock(489, 488),
                new AnimBlock(490, 488),
                new AnimBlock(491, 488),
                new AnimBlock(492, 488),
                new AnimBlock(493, 488),
            }, 

            // Lividstone
            new AnimBlock[]
            {
                new AnimBlock(495, 494),
                new AnimBlock(496, 494),
                new AnimBlock(497, 494),
                new AnimBlock(498, 494),
                new AnimBlock(499, 494),
                new AnimBlock(500, 494),
                new AnimBlock(501, 494),
                new AnimBlock(502, 494),
                new AnimBlock(503, 494),
                new AnimBlock(504, 494),
                new AnimBlock(506, 505),
            }, 

            // Enemy fish 1
            new AnimBlock[]
            {
                new AnimBlock(508, 507),
                new AnimBlock(509, 507),
            }, 

            // Enemy fish 2
            new AnimBlock[]
            {
                new AnimBlock(511, 510),
                new AnimBlock(512, 510),
            }, 

            // Pink Rabbid
            new AnimBlock[]
            {
                new AnimBlock(514, 513),
                new AnimBlock(515, 513),
                new AnimBlock(516, 513),
                new AnimBlock(517, 513),
                new AnimBlock(518, 513),
            }, 

            // Flying Rabbid
            new AnimBlock[]
            {
                new AnimBlock(520, 519),
                new AnimBlock(521, 519),
                new AnimBlock(522, 519),
                new AnimBlock(523, 519),
                new AnimBlock(524, 519),
                new AnimBlock(525, 519),
                new AnimBlock(527, 526),
            }, 

            // Green slime
            new AnimBlock[]
            {
                new AnimBlock(529, 528),
                new AnimBlock(530, 528),
                new AnimBlock(531, 528),
                new AnimBlock(532, 528),
                new AnimBlock(533, 528),
            }, 

            // Red slime
            new AnimBlock[]
            {
                new AnimBlock(535, 534),
                new AnimBlock(536, 534),
                new AnimBlock(537, 534),
                new AnimBlock(538, 534),
                new AnimBlock(539, 534),
            }, 

            // Unused Rabbid
            new AnimBlock[]
            {
                new AnimBlock(541, 540),
                new AnimBlock(542, 540),
                new AnimBlock(543, 540),
                new AnimBlock(544, 540),
                new AnimBlock(545, 540),
                new AnimBlock(546, 540),
            }, 

            // Chef
            new AnimBlock[]
            {
                new AnimBlock(548, 547),
                new AnimBlock(549, 547),
                new AnimBlock(550, 547),
                new AnimBlock(551, 547),
                new AnimBlock(552, 547),
                new AnimBlock(553, 547),
                new AnimBlock(554, 547),
                new AnimBlock(555, 547),
                new AnimBlock(557, 556),
            }, 

            // Unused sweets enemy
            new AnimBlock[]
            {
                new AnimBlock(559, 558),
            }, 

            // Enemy cake
            new AnimBlock[]
            {
                new AnimBlock(561, 560),
                new AnimBlock(562, 560),
                new AnimBlock(563, 560),
                new AnimBlock(564, 560),
                new AnimBlock(565, 560),
                new AnimBlock(567, 566),
            }, 

            // Enemy muffin
            new AnimBlock[]
            {
                new AnimBlock(569, 568),
                new AnimBlock(570, 568),
                new AnimBlock(572, 571),
                new AnimBlock(573, 571),
                new AnimBlock(574, 571),
                new AnimBlock(575, 571),
            }, 

            // Enemy lollipop
            new AnimBlock[]
            {
                new AnimBlock(577, 576),
                new AnimBlock(578, 576),
                new AnimBlock(579, 576),
                new AnimBlock(580, 576),
                new AnimBlock(581, 576),
            }, 

            // Rabbid with flying hair
            new AnimBlock[]
            {
                new AnimBlock(583, 582),
                new AnimBlock(584, 582),
                new AnimBlock(585, 582),
                new AnimBlock(586, 582),
                new AnimBlock(587, 582),
                new AnimBlock(588, 582),
            }, 

            // Rabbid guard
            new AnimBlock[]
            {
                new AnimBlock(590, 589),
                new AnimBlock(591, 589),
                new AnimBlock(592, 589),
                new AnimBlock(593, 589),
                new AnimBlock(594, 589),
            }, 

            // Rabbid robot
            new AnimBlock[]
            {
                new AnimBlock(596, 595),
                new AnimBlock(597, 595),
                new AnimBlock(598, 595),
                new AnimBlock(599, 595),
                new AnimBlock(600, 595),
                new AnimBlock(602, 601),
            }, 

            // Big gray Rabbid
            new AnimBlock[]
            {
                new AnimBlock(604, 603),
                new AnimBlock(605, 603),
                new AnimBlock(606, 603),
                new AnimBlock(607, 603),
            }, 

            // Scenery
            new AnimBlock[]
            {
                new AnimBlock(609, 608),
                new AnimBlock(611, 610),
                new AnimBlock(613, 612),
                new AnimBlock(614, 612),
                new AnimBlock(616, 615),
                new AnimBlock(617, 615),
                new AnimBlock(618, 610),
                new AnimBlock(619, 610),
                new AnimBlock(620, 615),
                new AnimBlock(621, 615),
                new AnimBlock(622, 615),
                new AnimBlock(624, 623),
                new AnimBlock(626, 625),
                new AnimBlock(628, 627),
                new AnimBlock(632, 631),
                new AnimBlock(634, 633),
                new AnimBlock(635, 633),
                new AnimBlock(636, 633),
                new AnimBlock(637, 633),
                new AnimBlock(638, 633),
                new AnimBlock(639, 633),
                new AnimBlock(660, 659),
                new AnimBlock(662, 661),
                new AnimBlock(664, 663),
                new AnimBlock(665, 663),
                new AnimBlock(671, 670),
                new AnimBlock(672, 673),
                new AnimBlock(679, 678),
                new AnimBlock(681, 680),
                new AnimBlock(683, 682),
            }, 

            // Hub portal
            new AnimBlock[]
            {
                new AnimBlock(630, 629),
            }, 

            // NPC
            new AnimBlock[]
            {
                new AnimBlock(641, 640),
                new AnimBlock(643, 642),
                new AnimBlock(645, 644),
                new AnimBlock(647, 646),
                new AnimBlock(649, 648),
                new AnimBlock(651, 650),
                new AnimBlock(653, 652),
                new AnimBlock(655, 654),
            }, 

            // Purple water
            new AnimBlock[]
            {
                new AnimBlock(657, 656),
                new AnimBlock(658, 656),
            }, 

            // Pink Rabbid
            new AnimBlock[]
            {
                new AnimBlock(667, 666),
                new AnimBlock(668, 666),
                new AnimBlock(669, 666),
                new AnimBlock(873, 666),
            }, 

            // Climbable
            new AnimBlock[]
            {
                new AnimBlock(675, 674),
                new AnimBlock(677, 676),
            }, 

            // Breakable ground
            new AnimBlock[]
            {
                new AnimBlock(685, 684),
                new AnimBlock(686, 684),
                new AnimBlock(688, 687),
            }, 

            // Switch
            new AnimBlock[]
            {
                new AnimBlock(691, 610),
                new AnimBlock(703, 702),
                new AnimBlock(705, 704),
                new AnimBlock(707, 706),
            }, 

            // Camera
            new AnimBlock[]
            {
                new AnimBlock(693, 692),
                new AnimBlock(694, 692),
                new AnimBlock(695, 692),
                new AnimBlock(696, 692),
                new AnimBlock(698, 697),
                new AnimBlock(699, 697),
                new AnimBlock(700, 697),
                new AnimBlock(701, 697),
            }, 

            // Enemy book 2
            new AnimBlock[]
            {
                new AnimBlock(709, 708),
            }, 

            // Doors
            new AnimBlock[]
            {
                new AnimBlock(710, 610),
                new AnimBlock(711, 610),
                new AnimBlock(712, 610),
                new AnimBlock(714, 713),
                new AnimBlock(716, 715),
            }, 

            // Hub doors
            new AnimBlock[]
            {
                new AnimBlock(720, 719),
                new AnimBlock(724, 723),
                new AnimBlock(728, 727),
                new AnimBlock(732, 731),
            }, 

            // Machine
            new AnimBlock[]
            {
                new AnimBlock(734, 733),
                new AnimBlock(735, 733),
            }, 

            // Targets
            new AnimBlock[]
            {
                new AnimBlock(737, 736),
                new AnimBlock(738, 736),
                new AnimBlock(740, 739),
                new AnimBlock(741, 739),
                new AnimBlock(743, 742),
                new AnimBlock(744, 742),
                new AnimBlock(746, 745),
                new AnimBlock(747, 745),
            }, 

            // Shooting range UI
            new AnimBlock[]
            {
                new AnimBlock(749, 748),
                new AnimBlock(751, 750),
                new AnimBlock(753, 752),
                new AnimBlock(755, 754),
                new AnimBlock(756, 754),
                new AnimBlock(757, 754),
                new AnimBlock(759, 758),
                new AnimBlock(761, 760),
                new AnimBlock(763, 762),
            }, 

            // Effects
            new AnimBlock[]
            {
                new AnimBlock(770, 769),
                new AnimBlock(772, 771),
                new AnimBlock(773, 320),
                new AnimBlock(775, 774),
                new AnimBlock(777, 776),
                new AnimBlock(778, 776),
                new AnimBlock(779, 776),
            }, 

            // Blue water
            new AnimBlock[]
            {
                new AnimBlock(781, 780),
                new AnimBlock(782, 780),
                new AnimBlock(783, 780),
            }, 

            // Target indicators
            new AnimBlock[]
            {
                new AnimBlock(785, 784),
                new AnimBlock(786, 784),
                new AnimBlock(787, 784),
                new AnimBlock(788, 784),
                new AnimBlock(789, 784),
            }, 

            // Prison boss
            new AnimBlock[]
            {
                new AnimBlock(809, 808),
                new AnimBlock(810, 808),
                new AnimBlock(811, 808),
                new AnimBlock(812, 808),
                new AnimBlock(813, 808),
                new AnimBlock(814, 808),
                new AnimBlock(815, 808),
                new AnimBlock(816, 808),
                new AnimBlock(817, 808),
                new AnimBlock(818, 808),
                new AnimBlock(819, 808),
                new AnimBlock(820, 808),
                new AnimBlock(821, 808),
                new AnimBlock(822, 808),
                new AnimBlock(823, 808),
            }, 

            // Prison boss platform
            new AnimBlock[]
            {
                new AnimBlock(825, 824),
                new AnimBlock(826, 824),
            }, 

            // Big Rabbid boss
            new AnimBlock[]
            {
                new AnimBlock(828, 827),
                new AnimBlock(829, 827),
                new AnimBlock(830, 827),
                new AnimBlock(831, 827),
                new AnimBlock(832, 827),
                new AnimBlock(833, 827),
                new AnimBlock(834, 827),
                new AnimBlock(835, 827),
                new AnimBlock(836, 827),
                new AnimBlock(837, 827),
                new AnimBlock(838, 827),
                new AnimBlock(839, 827),
                new AnimBlock(840, 827),
                new AnimBlock(841, 827),
                new AnimBlock(842, 827),
                new AnimBlock(843, 827),
                new AnimBlock(844, 827),
                new AnimBlock(845, 827),
            }, 

            // Bombs
            new AnimBlock[]
            {
                new AnimBlock(847, 846),
                new AnimBlock(849, 848),
                new AnimBlock(851, 850),
                new AnimBlock(853, 852),
                new AnimBlock(855, 854),
            }, 

            // Boss minions
            new AnimBlock[]
            {
                new AnimBlock(857, 856),
                new AnimBlock(858, 856),
                new AnimBlock(859, 856),
                new AnimBlock(872, 871),
            }, 

            // Boss organic thing
            new AnimBlock[]
            {
                new AnimBlock(861, 860),
                new AnimBlock(862, 860),
                new AnimBlock(863, 860),
                new AnimBlock(864, 860),
                new AnimBlock(865, 860),
            }, 

            // Boss cookie projectiles
            new AnimBlock[]
            {
                new AnimBlock(867, 866),
                new AnimBlock(868, 866),
            }, 

            // Spikes
            new AnimBlock[]
            {
                new AnimBlock(870, 869),
            }, 

            // Final boss
            new AnimBlock[]
            {
                new AnimBlock(875, 874),
                new AnimBlock(876, 874),
                new AnimBlock(877, 874),
                new AnimBlock(878, 874),
                new AnimBlock(879, 874),
                new AnimBlock(880, 889),
                new AnimBlock(881, 889),
                new AnimBlock(883, 882),
                new AnimBlock(884, 882),
                new AnimBlock(885, 882),
                new AnimBlock(886, 882),
                new AnimBlock(887, 882),
                new AnimBlock(888, 882),
                new AnimBlock(890, 874),
                new AnimBlock(891, 874),
                new AnimBlock(892, 874),
                new AnimBlock(894, 893),
                new AnimBlock(895, 893),
                new AnimBlock(896, 893),
                new AnimBlock(897, 893),
                new AnimBlock(898, 893),
                new AnimBlock(899, 893),
                new AnimBlock(900, 893),
                new AnimBlock(901, 893),
                new AnimBlock(902, 893),
                new AnimBlock(903, 893),
                new AnimBlock(904, 893),
            }, 
        };

        public class AnimBlock
        {
            public AnimBlock(int animBlockIndex, int palBlockIndex, byte animSpeed = 5, byte subPalette = 0)
            {
                AnimBlockIndex = animBlockIndex;
                PalBlockIndex = palBlockIndex;
                AnimSpeed = animSpeed;
                SubPalette = subPalette;
            }

            public int AnimBlockIndex { get; }
            public int PalBlockIndex { get; }
            public byte AnimSpeed { get; }
            public byte SubPalette { get; }
        }
    }
}